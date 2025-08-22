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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Globalization;
using System.Text.Json;

namespace CSIDE.Data.Interceptors;

public class AuditInterceptor(ILogger<AuditInterceptor> logger,
                              ICurrentUserService currentUserService) : SaveChangesInterceptor, IAuditInterceptor
{
    private bool _isSavingChanges;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (_isSavingChanges) return result;

        var context = eventData.Context;
        if (context == null) return result;

        LogChanges(context).Wait();
        return result;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (_isSavingChanges) return result;

        var context = eventData.Context;
        if (context == null) return result;

        await LogChanges(context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task LogChanges(DbContext context)
    {
        try
        {
            var auditLogs = new List<AuditLog>();
            var addedEntities = new List<EntityEntry>();
            // Get user information asynchronously (preferred for Blazor operations)
            var userId = await currentUserService.GetUserIdAsync();
            var userName = await currentUserService.GetUserNameAsync() ?? "Unknown user";

            var jsonSerializerOpts = new JsonSerializerOptions().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (ShouldAudit(entry))
                {
                    var keyColumn = entry.Metadata.FindPrimaryKey();
                    if (keyColumn == null)
                    {
                        logger.LogWarning("Entity {EntityName} does not have a primary key and will not be audited", entry.Entity.GetType().Name);
                        continue;
                    }

                    if (entry.State == EntityState.Added)
                    {
                        addedEntities.Add(entry);
                    }
                    else
                    {
                        auditLogs.Add(CreateAuditLog(entry, keyColumn, userId, userName, jsonSerializerOpts, context));
                    }
                }
            }

            if (auditLogs.Count != 0)
            {
                context.Set<AuditLog>().AddRange(auditLogs);
            }

            _isSavingChanges = true;
            await context.SaveChangesAsync();
            auditLogs.Clear();
            _isSavingChanges = false;

            foreach (var entry in addedEntities)
            {
                var keyColumn = entry.Metadata.FindPrimaryKey();
                if (keyColumn == null)
                {
                    logger.LogWarning("Entity {EntityName} does not have a primary key and will not be audited", entry.Entity.GetType().Name);
                    continue;
                }

                auditLogs.Add(CreateAuditLog(entry, keyColumn, userId, userName, jsonSerializerOpts, context, EntityState.Added));
            }

            if (auditLogs.Count != 0)
            {
                context.Set<AuditLog>().AddRange(auditLogs);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred adding an audit log entry");
        }
    }

    private static AuditLog CreateAuditLog(EntityEntry entry, IKey keyColumn, string? userId, string userName, JsonSerializerOptions jsonSerializerOpts, DbContext context, EntityState? stateOverride = null)
    {
        var primaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).FirstOrDefault()?.ToString() ?? string.Empty;
        var secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).Skip(1).FirstOrDefault()?.ToString() ?? string.Empty;


        //manual interventions for entities that don't follow the standard pattern
        //RoW Statements, make the RouteId the primary entity id and the version the secondary entity id
        if (entry.Entity is Models.RightsOfWay.Statement statement)
        {
            primaryEntityId = statement.RouteId;
            secondaryEntityId = $"v{statement.Version}";
        }
        //Comments, make the JobId the primary entity ID and the CommentId the secondary entity id
        if (entry.Entity is Comment comment)
        {
            secondaryEntityId = comment.JobId.ToString(CultureInfo.InvariantCulture);
            (primaryEntityId, secondaryEntityId) = (secondaryEntityId, primaryEntityId);
        }
        if (entry.Entity is PPOEvent ppoEvent)
        {
            secondaryEntityId = ppoEvent.ApplicationId.ToString(CultureInfo.InvariantCulture);
            (primaryEntityId, secondaryEntityId) = (secondaryEntityId, primaryEntityId);
        }
        if (entry.Entity is DMMOEvent dmmoEvent)
        {
            secondaryEntityId = dmmoEvent.ApplicationId.ToString(CultureInfo.InvariantCulture);
            (primaryEntityId, secondaryEntityId) = (secondaryEntityId, primaryEntityId);
        }

        //Landowner deposits use composite keys, which work differently again
        if (entry.Entity is LandownerDeposit)
        {
            //The base table uses a composite key, so the primaryEntityId should be made of the first two 
            primaryEntityId = $"{primaryEntityId}/{secondaryEntityId}";
            secondaryEntityId = null;
        }
        if (entry.Entity is LandownerDepositAddress)
        {
            //for linked tables, the primaryEntityId should be the first two concatenated together, and the secondaryEntityId should be 3rd (or last) key column
            primaryEntityId = $"{primaryEntityId}/{secondaryEntityId}";
            //get the key for the linked entity. Assumption that this will be just 1 key column, and not another composite
            secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).LastOrDefault()?.ToString() ?? string.Empty;
        }
        if (entry.Entity is LandownerDepositEvent)
        {
            primaryEntityId = $"{primaryEntityId}/{secondaryEntityId}";
            secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).LastOrDefault()?.ToString() ?? string.Empty;
        }
        if (entry.Entity is LandownerDepositContact)
        {
            primaryEntityId = $"{primaryEntityId}/{secondaryEntityId}";
            secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).LastOrDefault()?.ToString() ?? string.Empty;
        }
        if (entry.Entity is LandownerDepositMedia)
        {
            primaryEntityId = $"{primaryEntityId}/{secondaryEntityId}";
            secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).LastOrDefault()?.ToString() ?? string.Empty;
        }
        if (entry.Entity is LandownerDepositParish)
        {
            primaryEntityId = $"{primaryEntityId}/{secondaryEntityId}";
            secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).LastOrDefault()?.ToString() ?? string.Empty;
        }
        if (entry.Entity is LandownerDepositType)
        {
            secondaryEntityId = $"{primaryEntityId}/{secondaryEntityId}";
            secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).LastOrDefault()?.ToString() ?? string.Empty;
        }

        //infra bridge details, make the InfraId the primary entity ID and the BridgeDetailsId the secondary entity id
        if (entry.Entity is InfrastructureBridgeDetails infrastructureBridgeDetails)
        {
            secondaryEntityId = infrastructureBridgeDetails.InfrastructureId.ToString(CultureInfo.InvariantCulture);
            (primaryEntityId, secondaryEntityId) = (secondaryEntityId, primaryEntityId);
        }
        //DMMO Order, make the ApplicationId the primary entity ID and the OrderId the secondary entity id
        if (entry.Entity is DMMOOrder order)
        {
            secondaryEntityId = order.ApplicationId.ToString(CultureInfo.InvariantCulture);
            (primaryEntityId, secondaryEntityId) = (secondaryEntityId, primaryEntityId);
        }
        //Media and Contacts, make the primary entity id the id of the parent entity and the secondary entity id the id of the child entity
        //This allows us to track the deletion of child records when a parent with delete cascade rules is deleted
        //It is a little bit inefficient but is rarely used and allows for richer details in the audit log
        if ((entry.Entity is Media or Contact) && entry.State == EntityState.Deleted)
        {
            var childEntities = GetChildEntities(context, entry);
            // Media/Contacts should only ever have one child entity
            if (childEntities.Where(c => c.State is EntityState.Deleted).Take(2).Count() == 1)
            {
                var childKeyColumn = childEntities[0].Metadata.FindPrimaryKey();
                if (childKeyColumn != null)
                {
                    primaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).FirstOrDefault()?.ToString() ?? string.Empty;
                    secondaryEntityId = keyColumn.Properties.Select(p => entry.Property(p.Name).CurrentValue).Skip(1).SingleOrDefault()?.ToString() ?? string.Empty;
                }
            }

        }

        var changedPropertiesOld = entry.Properties
            .Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
            .ToDictionary(p => p.Metadata.Name, p => ConvertGeometryToSerializableFormat(p.OriginalValue!), StringComparer.Ordinal);

        var changedPropertiesNew = entry.Properties
            .Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
            .ToDictionary(p => p.Metadata.Name, p => ConvertGeometryToSerializableFormat(p.CurrentValue!), StringComparer.Ordinal);

        if (entry.State == EntityState.Added || stateOverride == EntityState.Added)
        {
            changedPropertiesNew = entry.Properties
                    .ToDictionary(p => p.Metadata.Name, p => ConvertGeometryToSerializableFormat(p.CurrentValue!), StringComparer.Ordinal);
        }
        if (entry.State == EntityState.Deleted || stateOverride == EntityState.Deleted)
        {
            changedPropertiesOld = entry.Properties
                    .ToDictionary(p => p.Metadata.Name, p => ConvertGeometryToSerializableFormat(p.OriginalValue!), StringComparer.Ordinal);
        }

        var oldPropertiesJson = JsonSerializer.SerializeToDocument(changedPropertiesOld, jsonSerializerOpts);
        var newPropertiesJson = JsonSerializer.SerializeToDocument(changedPropertiesNew, jsonSerializerOpts);

        return new AuditLog
        {
            EntityName = entry.Entity.GetType().Name,
            ChangeType = (stateOverride ?? entry.State).ToString(),
            EntityId = primaryEntityId,
            SecondaryEntityId = secondaryEntityId,
            NewValues = newPropertiesJson,
            OldValues = oldPropertiesJson,
            UserId = userId,
            UserName = userName,
        };
    }

    private static List<EntityEntry> GetChildEntities(DbContext context, EntityEntry parentEntry)
    {
        var childEntries = new List<EntityEntry>();
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

    private static bool ShouldAudit(EntityEntry entry) => entry.Entity is not AuditLog &&
        entry.Entity is not DMMOParish &&
        entry.Entity is not LandownerDepositParish &&
        entry.Entity is not DMMOMediaType &&
        entry.Entity is not LandownerDepositMediaType &&
        entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted &&
        !(entry.Entity is Media && entry.State is EntityState.Modified);

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
