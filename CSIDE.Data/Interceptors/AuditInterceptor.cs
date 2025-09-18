using CSIDE.Data.Models.Audit;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using CSIDE.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Globalization;
using System.Text.Json;

namespace CSIDE.Data.Interceptors;

internal class AuditInterceptor : ISaveChangesInterceptor
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    private static bool DontAudit(EntityEntry entry) => entry.Entity is AuditLog or DMMOParish or LandownerDepositParish or DMMOMediaType or LandownerDepositMediaType || entry.State is EntityState.Unchanged or EntityState.Detached;
    private bool _isSavingChanges;
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for audit logs.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (_isSavingChanges) return result;
        await LogChanges(eventData);
        return await ValueTask.FromResult(result);
    }

    private async Task LogChanges(DbContextEventData eventData)
    {
        
        if (eventData.Context is not ApplicationDbContext context) return;
        var logger = context.GetService<ILogger<AuditInterceptor>>();

        try
        {

            var currentUserService = context.GetService<ICurrentUserService>();
            var jsonSerializerOpts = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            // Build the audit logs to add at the end
            List<AuditLog> auditLogs = [];
            var addedEntities = new List<EntityEntry>();

            if (!currentUserService.IsAuthenticated)
            {
                logger.LogError("AuditInterceptor - User is not authenticated.");
            }
            var userId = currentUserService.UserId;
            var userName = currentUserService.UserName;

            // Loop through the changes
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (DontAudit(entry)) continue;
                if (entry.Entity is Media && entry.State is EntityState.Modified) continue;


                //add the modification logs first
                if (entry.State == EntityState.Added)
                {
                    addedEntities.Add(entry);
                }
                else
                {
                    var auditLog = entry.Entity is Media or Contact
                    ? CreateMediaOrContactAuditLog(entry, userId, userName, logger, context)
                    : CreateAuditLog(entry, userId, userName, logger);
                    if (auditLog is not null)
                    {
                        auditLogs.Add(auditLog);
                    }
                }
            }
            //save the changes
            if (auditLogs.Count != 0)
            {
                context.Set<AuditLog>().AddRange(auditLogs);
            }

            _isSavingChanges = true;
            await context.SaveChangesAsync();
            auditLogs.Clear();
            _isSavingChanges = false;
            //then add the addition logs
            foreach (var entity in addedEntities)
            {
                var auditLog = entity.Entity is Media or Contact
                ? CreateMediaOrContactAuditLog(entity, userId, userName, logger, context, EntityState.Added)
                : CreateAuditLog(entity, userId, userName, logger, EntityState.Added);
                if (auditLog is not null)
                {
                    auditLogs.Add(auditLog);
                }
            }

            if (auditLogs.Count != 0)
            {
                context.Set<AuditLog>().AddRange(auditLogs);
            }
        }catch(Exception ex)
        {
            logger.LogError(ex,"An error occurred adding an audit log entry");
        }
    }

    private AuditLog? CreateAuditLog(EntityEntry entry, string userId, string userName, ILogger<AuditInterceptor> logger, EntityState? entityStateOverride = null)
    {
        // Get the primary key / column
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey == null)
        {
            logger.LogWarning("Entity {EntityName} does not have a primary key and will not be audited", entry.Entity.GetType().Name);
            return null;
        }

        var (primaryId, secondaryId) = GetKeyColumnIds(primaryKey, entry);
        var newPropertiesJson = GetNewPropertiesJson(entry, entityStateOverride ?? entry.State);
        var oldPropertiesJson = GetOldPropertiesJson(entry, entityStateOverride ?? entry.State);

        return new AuditLog
        {
            EntityName = entry.Entity.GetType().Name,
            ChangeType = (entityStateOverride ?? entry.State).ToString(),
            EntityId = primaryId,
            SecondaryEntityId = secondaryId,
            NewValues = newPropertiesJson,
            OldValues = oldPropertiesJson,
            UserId = userId,
            UserName = userName,
        };
    }

    /// <summary>
    ///     <para>Media and Contacts, make the primary entity id the id of the parent entity and the secondary entity id the id of the child entity.</para>
    ///     <para>This allows us to track the deletion of child records when a parent with delete cascade rules is deleted.</para>
    ///     <para>It is a little bit inefficient but is rarely used and allows for richer details in the audit log.</para>
    /// </summary>
    private AuditLog? CreateMediaOrContactAuditLog(
        EntityEntry entry,
        string userId,
        string userName,
        ILogger<AuditInterceptor> logger,
        ApplicationDbContext context,
        EntityState? entityStateOverride = null)
    {
        // Safeguard - should never hit this as we check before calling
        if (!(entry.Entity is Media or Contact)) return null;

        // Get the primary key / column
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey == null)
        {
            logger.LogWarning("Entity {EntityName} does not have a primary key and will not be audited", entry.Entity.GetType().Name);
            return null;
        }

        string? primaryId = null;
        string? secondaryId = null;

        if (primaryId == null || secondaryId == null)
        {
            (primaryId, secondaryId) = GetDefaultKeyColumnIds(primaryKey, entry);
        }

        var newPropertiesJson = GetNewPropertiesJson(entry, entityStateOverride ?? entry.State);
        var oldPropertiesJson = GetOldPropertiesJson(entry, entityStateOverride ?? entry.State);

        return new AuditLog
        {
            EntityName = entry.Entity.GetType().Name,
            ChangeType = (entityStateOverride ?? entry.State).ToString(),
            EntityId = primaryId,
            SecondaryEntityId = secondaryId,
            NewValues = newPropertiesJson,
            OldValues = oldPropertiesJson,
            UserId = userId,
            UserName = userName,
        };
    }

    /// <summary>
    ///     Work out the EntityId and SecondaryEntityId values based on each entity.
    /// </summary>
    private static (string primaryId, string secondaryId) GetKeyColumnIds(IKey primaryKey, EntityEntry entry)
    {
        var (primaryId, secondaryId) = GetDefaultKeyColumnIds(primaryKey, entry);

        // Manual interventions for entities that don't follow the standard pattern

        // Comments, make the JobId the primary entity ID and the CommentId the secondary entity ID
        if (entry.Entity is Comment comment)
        {
            secondaryId = primaryId;
            primaryId = comment.JobId.ToString(CultureInfo.InvariantCulture);
        }

        // DMMO Event, make the ApplicationId the primary entity ID and the DMMOEventId the secondary entity ID
        if (entry.Entity is DMMOEvent dmmoEvent)
        {
            secondaryId = primaryId;
            primaryId = dmmoEvent.DMMOApplicationId.ToString(CultureInfo.InvariantCulture);
        }

        // DMMO Order, make the ApplicationId the primary entity ID and the OrderId the secondary entity ID
        if (entry.Entity is DMMOOrder order)
        {
            secondaryId = primaryId;
            primaryId = order.DMMOApplicationId.ToString(CultureInfo.InvariantCulture);
        }

        // Infrastructure bridge details, make the InfraId the primary entity ID and the BridgeDetailsId the secondary entity ID
        if (entry.Entity is InfrastructureBridgeDetails infrastructureBridgeDetails)
        {
            secondaryId = primaryId;
            primaryId = infrastructureBridgeDetails.InfrastructureId.ToString(CultureInfo.InvariantCulture);
        }

        // Landowner deposits use composite keys
        if (entry.Entity is LandownerDeposit)
        {
            // The base table uses a composite key, so the primary entity ID should be made of the first two
            // For linked tables, the primary entity ID should be the first two concatenated together
            primaryId = $"{primaryId}/{secondaryId}";
            secondaryId = string.Empty;
        }
        if (entry.Entity is LandownerDepositEvent landownerDepositEvent)
        {
            secondaryId = primaryId;
            primaryId = $"{landownerDepositEvent.LandownerDepositId}/{landownerDepositEvent.LandownerDepositSecondaryId}";
        }
        if (entry.Entity is LandownerDepositAddress or LandownerDepositContact or LandownerDepositMedia or LandownerDepositParish or LandownerDepositType)
        {
            primaryId = $"{primaryId}/{secondaryId}";
            // the secondary entity ID should be 3rd (or last) key column. Assumption that this will be just 1 key column, and not another composite
            secondaryId = GetLastKeyColumnId(primaryKey, entry);
        }

        // RightsOfWay Statements, make the RouteId the primary entity ID and the version the secondary entity ID
        if (entry.Entity is Models.RightsOfWay.Statement statement)
        {
            primaryId = statement.RouteId;
            secondaryId = $"v{statement.Version}";
        }

        // PPO Order, make the ApplicationId the primary entity ID and the OrderId the secondary entity ID
        if (entry.Entity is PPOOrder ppoOrder)
        {
            secondaryId = primaryId;
            primaryId = ppoOrder.PPOApplicationId.ToString(CultureInfo.InvariantCulture);
        }
        // PPO Events, make the ApplicationId the primary entity ID and the PPOEventId the secondary entity ID
        if (entry.Entity is PPOEvent ppoEvent)
        {
            secondaryId = primaryId;
            primaryId = ppoEvent.PPOApplicationId.ToString(CultureInfo.InvariantCulture);
        }

        return (primaryId, secondaryId);
    }

    /// <summary>
    /// Default values for EntityId and SecondaryEntityId
    /// </summary>
    private static (string primaryId, string secondaryId) GetDefaultKeyColumnIds(IKey key, EntityEntry entry)
    {
        var property = key.Properties.Select(p => entry.Property(p.Name).CurrentValue);
        if (property == null || !property.Any())
        {
            return (string.Empty, string.Empty);
        }

        var primaryId = property.FirstOrDefault()?.ToString() ?? string.Empty;
        var secondaryId = property.Skip(1).FirstOrDefault()?.ToString() ?? string.Empty;

        return (primaryId, secondaryId);
    }

    /// <summary>
    /// Get the last key column id
    /// </summary>
    private static string GetLastKeyColumnId(IKey key, EntityEntry entry)
    {
        return key.Properties
            .Select(p => entry.Property(p.Name).CurrentValue)
            .Skip(1)
            .FirstOrDefault()?.ToString()
            ?? string.Empty;
    }

    /// <summary>
    /// Get the added or changed properties as a JsonDocument
    /// </summary>
    /// <remarks>Uses the CurrentValue property for ConvertGeometryToSerializableFormat</remarks>
    private JsonDocument GetNewPropertiesJson(EntityEntry entry, EntityState entityState)
    {
        IDictionary<string, object> properties;

        if (entityState == EntityState.Added)
        {
            // On add, get new/current properties
            properties = entry.Properties.ToDictionary(
                p => p.Metadata.Name,
                p => ConvertGeometryToSerializableFormat(p.CurrentValue!),
                StringComparer.Ordinal
            );
        }
        else
        {
            // Get changed new/current properties
            properties = entry.Properties
                .Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
                .ToDictionary(
                    p => p.Metadata.Name,
                    p => ConvertGeometryToSerializableFormat(p.CurrentValue!),
                    StringComparer.Ordinal
                );
        }

        return JsonSerializer.SerializeToDocument(properties, _jsonSerializerOptions);
    }

    /// <summary>
    /// Get the deleted or changed properties as a JsonDocument
    /// </summary>
    /// <remarks>Uses the OriginalValue property for ConvertGeometryToSerializableFormat</remarks>
    private JsonDocument GetOldPropertiesJson(EntityEntry entry, EntityState entityState)
    {
        IDictionary<string, object> properties;

        if (entityState == EntityState.Deleted)
        {
            // On delete, get original/old values
            properties = entry.Properties.ToDictionary(
                p => p.Metadata.Name,
                p => ConvertGeometryToSerializableFormat(p.OriginalValue!),
                StringComparer.Ordinal
            );
        }
        else
        {
            // Get changed original/old values
            properties = entry.Properties
                .Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
                .ToDictionary(
                    p => p.Metadata.Name,
                    p => ConvertGeometryToSerializableFormat(p.OriginalValue!),
                    StringComparer.Ordinal
                );
        }

        return JsonSerializer.SerializeToDocument(properties, _jsonSerializerOptions);
    }

    private static ICollection<EntityEntry> GetChildEntities(ApplicationDbContext context, EntityEntry parentEntry)
    {
        ICollection<EntityEntry> childEntries = [];
        var entityType = parentEntry.Metadata;

        foreach (var navigation in entityType.GetNavigations())
        {
            if (navigation.IsCollection)
            {
                var relatedEntities = context.Entry(parentEntry.Entity).Collection(navigation.Name).CurrentValue;
                if (relatedEntities != null)
                {
                    foreach (var relatedEntity in relatedEntities)
                    {
                        var relatedEntry = context.Entry(relatedEntity);
                        if (relatedEntry.State != EntityState.Detached)
                        {
                            childEntries.Add(relatedEntry);
                        }
                    }
                }
            }
            else
            {
                var relatedEntity = context.Entry(parentEntry.Entity).Reference(navigation.Name).CurrentValue;
                if (relatedEntity != null)
                {
                    var relatedEntry = context.Entry(relatedEntity);
                    if (relatedEntry.State != EntityState.Detached)
                    {
                        childEntries.Add(relatedEntry);
                    }
                }
            }
        }

        return childEntries;
    }

    private static object ConvertGeometryToSerializableFormat(object value)
    {
        if (value is Geometry geometry)
        {
            var wktWriter = new WKTWriter();
            return wktWriter.Write(geometry);
        }
        return value;
    }
}
