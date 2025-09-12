using CSIDE.Data.Models.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Interceptors;

internal class MaintenanceJobInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for maintenance jobs.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await UpdateMaintenanceJob(eventData, cancellationToken);
        return await ValueTask.FromResult(result);
    }

    private static async Task UpdateMaintenanceJob(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Context is not ApplicationDbContext context) return;

        foreach (var entry in context.ChangeTracker.Entries<Job>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.ParishId = await GetParishIdForGeom(context, entry.Entity.Geom, cancellationToken);
                if (entry.State is EntityState.Added)
                {
                    entry.Entity.MaintenanceTeamId = await GetMaintenanceTeamIdForGeom(context, entry.Entity.Geom, cancellationToken);
                }
            }
        }
    }

    /// <summary>
    /// Gets the Parish ID for a new or updated job based on a spatial contains query
    /// </summary>
    private static async Task<int?> GetParishIdForGeom(ApplicationDbContext context, Point? geom, CancellationToken cancellationToken)
    {
        if (geom == null) return null;

        return await context.Parishes
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(p => p.Geom.Contains(geom))
            .Select(p => p.ParishId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the Maintenance Team ID for a new job based on a spatial contains query
    /// </summary>
    private static async Task<int?> GetMaintenanceTeamIdForGeom(ApplicationDbContext context, Point? geom, CancellationToken cancellationToken)
    {
        if (geom == null) return null;

        return await context.MaintenanceTeams
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(t => t.Geom.Contains(geom))
            .Select(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
