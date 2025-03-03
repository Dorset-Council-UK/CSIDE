using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CSIDE.Services;

/// <summary>
/// Helper methods for when dealing with Rights of Way (Route) data
/// </summary>
public class RightsOfWayHelperService(IDbContextFactory<ApplicationDbContext> contextFactory) : IRightsOfWayHelperService
{
    /// <summary>
    /// Get the nearest route to a given location
    /// </summary>
    /// <param name="location">The NetTopologySuite Point that represents the location</param>
    /// <param name="maxDistance">The max distance to get a route from in metres. Defaults to 20.</param>
    /// <returns>A RightsOfWay.Route, or null if none found</returns>
    public async Task<Data.Models.RightsOfWay.Route?> GetNearestRouteAsync(Point location, int maxDistance = 20)
    {
        await using var context = contextFactory.CreateDbContext();
        return await context.Routes
            .Where(r => r.Geom.Distance(location) < maxDistance)
            .OrderBy(r => r.Geom.Distance(location))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> RouteExistsAsync(string RouteCode)
    {
        await using var context = contextFactory.CreateDbContext();
        return await context.Routes.AnyAsync(r => r.RouteCode == RouteCode);


    }
}
