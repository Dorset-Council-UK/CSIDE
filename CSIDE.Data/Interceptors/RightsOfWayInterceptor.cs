using CSIDE.Data.Models.RightsOfWay;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Interceptors;

internal class RightsOfWayInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for rights of way entities.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await UpdateRightsOfWay(eventData, cancellationToken);
        return await ValueTask.FromResult(result);
    }

    private static async Task UpdateRightsOfWay(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Context is not ApplicationDbContext context) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                if (entry.Entity is Route route)
                {
                    route.ParishId = await GetParishIdForGeom(context, route.Geom, cancellationToken);
                    route.MaintenanceTeamId = await GetTeamIdForRouteForGeom(context, route.Geom, cancellationToken);
                    await FixClosureData(context, route, cancellationToken);
                }
                else if (entry.Entity is Statement statement)
                {
                    statement.Version = await NextVersionNumber(context, statement.RouteId, cancellationToken);
                }
            }
        }
    }

    /// <summary>
    /// Gets the Parish ID of a new or updated job based on a spatial contains query
    /// </summary>
    private static async Task<int?> GetParishIdForGeom(ApplicationDbContext context, MultiLineString geom, CancellationToken cancellationToken)
    {
        return await context.Parishes
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(p => p.Geom.Intersects(geom))
            .OrderByDescending(p => p.Geom.Intersection(geom).Length)
            .Select(p => p.ParishId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the Maintenance Team ID of a newly created job based on a spatial contains query
    /// </summary>
    private static async Task<int?> GetTeamIdForRouteForGeom(ApplicationDbContext context, MultiLineString geom, CancellationToken cancellationToken)
    {
        return await context.MaintenanceTeams
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(t => t.Geom.Contains(geom))
            .Select(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Makes changes to the closure data of a Right of Way based on operational status
    /// </summary>
    private static async Task FixClosureData(ApplicationDbContext context, Route route, CancellationToken cancellationToken)
    {
        var operationalStatus = await context.RouteOperationalStatuses
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(os => os.Id == route.OperationalStatusId)
            .Select(os => new { os.Id, os.IsClosed })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (operationalStatus == null) return;

        if (!operationalStatus.IsClosed)
        {
            route.ClosureStartDate = null;
            route.ClosureEndDate = null;
            route.ClosureIsIndefinite = false;
        }
        if (operationalStatus.IsClosed && route.ClosureIsIndefinite)
        {
            route.ClosureEndDate = null;
        }
    }

    private static async Task<int> NextVersionNumber(ApplicationDbContext context, string routeId, CancellationToken cancellationToken)
    {
        var highestVersionNumber = await context.Statements
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(s => s.RouteId == routeId)
            .Select(s => (int?)s.Version)
            .MaxAsync(cancellationToken)
            .ConfigureAwait(false) ?? 0;

        return highestVersionNumber + 1;
    }
}
