using CSIDE.Data.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Interceptors;

internal class InfrastructureItemInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for infrastructure items.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await UpdateInfrastructureItem(eventData, cancellationToken);
        return await ValueTask.FromResult(result);
    }

    private static async Task UpdateInfrastructureItem(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Context is not ApplicationDbContext context) return;

        var infrastructureItems = context.ChangeTracker.Entries<InfrastructureItem>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified)
            .Select(entry => entry.Entity)
            .ToList();

        foreach (var infrastructureItem in infrastructureItems)
        {
            infrastructureItem.ParishId = await GetParishIdForGeom(context, infrastructureItem.Geom, cancellationToken);
            infrastructureItem.MaintenanceTeamId = await GetMaintenanceTeamIdForGeom(context, infrastructureItem.Geom, cancellationToken);
        }
    }

    private static async Task<int?> GetParishIdForGeom(ApplicationDbContext context, Point? geom, CancellationToken cancellationToken)
    {
        if (geom == null) return null;

        var parishId = await context.Parishes
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(p => p.Geom.Contains(geom))
            .Select(p => p.ParishId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (parishId == 0) return null;

        return parishId;
    }

    private static async Task<int?> GetMaintenanceTeamIdForGeom(ApplicationDbContext context, Point? geom, CancellationToken cancellationToken)
    {
        if (geom == null) return null;

        var maintenanceTeamId = await context.MaintenanceTeams
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(t => t.Geom.Contains(geom))
            .Select(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (maintenanceTeamId == 0) return null;

        return maintenanceTeamId;
    }
}
