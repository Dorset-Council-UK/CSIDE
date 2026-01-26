using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NodaTime;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace CSIDE.Data.Services;

/// <summary>
/// Helper methods for when dealing with Rights of Way (Route) data
/// </summary>
public class RightsOfWayService(IDbContextFactory<ApplicationDbContext> contextFactory) : IRightsOfWayService
{
    // Dictionary to map sort strings to property expressions for better performance
    private static readonly Dictionary<string, Expression<Func<Route, object>>> SortExpressions = new()
        {
            { "RouteId", x => x.RouteCode },
            { "Parish", x => x.Parish.Name ?? string.Empty },
            { "MaintenanceTeam", x => x.MaintenanceTeam.Name ?? string.Empty },
            { "Name", x => x.Name ?? string.Empty },
            { "OperationalStatus", x => x.OperationalStatus.Name ?? string.Empty },
            { "RouteType", x => x.RouteType.Name ?? string.Empty },


        };
    public async Task<Route?> GetRouteByCode(string routeCode, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Routes
            .FirstOrDefaultAsync(r => r.RouteCode == routeCode.ToUpper(), cancellationToken: ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Get the nearest route to a given location
    /// </summary>
    /// <param name="location">The NetTopologySuite Point that represents the location</param>
    /// <param name="maxDistance">The max distance to get a route from in metres. Defaults to 20.</param>
    /// <returns>A RightsOfWay.Route, or null if none found</returns>
    public async Task<Route?> GetNearestRoute(Point location, int maxDistance = 20, CancellationToken ct = default)
    {
        var results = await GetNearestRoutes(location, maxDistance, 1, ct);
        return results.Take(1).FirstOrDefault();
    }

    public async Task<ICollection<Route>> GetNearestRoutes(Geometry geometry, int maxDistance = 50, int maxRoutes = 50, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Routes
                .Where(i => i.Geom.IsWithinDistance(geometry, maxDistance))
                .OrderBy(i => i.Geom.Distance(geometry))
                .Take(maxRoutes)
                .ToListAsync(cancellationToken: ct)
                .ConfigureAwait(false);
    }

    public async Task<ICollection<Geometry>> GetRoutesIntersecting(Polygon bboxPolygon, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Routes
                .Where(i => i.Geom.Intersects(bboxPolygon))
                .Select(r => r.Geom)
                .ToArrayAsync(cancellationToken: ct)
                .ConfigureAwait(false);
    }
    public async Task<ICollection<Statement>> GetStatementsByRouteId(string routeId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Statements
                .Where(s => s.RouteId == routeId)
                .ToArrayAsync(cancellationToken: ct)
                .ConfigureAwait(false);
    }

    public async Task<PagedResult<Route>> GetRoutesBySearchParameters(
        string? RouteId,
        string? Name,
        string[]? ParishIds,
        string? ParishId,
        string? MaintenanceTeamId,
        string? OperationalStatusId,
        string? RouteTypeId,
        string? OrderBy = "RouteId",
        ListSortDirection OrderDirection = ListSortDirection.Descending,
        int PageNumber = 1,
        int PageSize = IRightsOfWayService.DefaultPageSize,
        CancellationToken ct = default)
    {
        var take = PageSize < 1 ? ILandownerDepositService.DefaultPageSize : PageSize;
        var skip = PageNumber < 1 ? 0 : (PageNumber - 1) * take;
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var query = context.Routes.AsQueryable();

        if (RouteId is not null)
        {
            query = query.Where(j => j.RouteCode == RouteId.ToUpper());
        }
        if (Name is not null)
        {
            query = query.Where(j => EF.Functions.ILike(j.Name!, $"%{Name}%"));
        }
        if (ParishIds is not null && ParishIds.Length != 0)
        {
            var parsedParishIds = ParishIds
                .Where(id => int.TryParse(id, CultureInfo.InvariantCulture, out _))
                .Select(id => int.Parse(id, CultureInfo.InvariantCulture))
                .ToList();
            if (parsedParishIds.Count != 0)
            {
                query = query.Where(j => j.ParishId != null && parsedParishIds.Contains(j.ParishId.Value));
            }
        }
        else if (ParishId is not null && int.TryParse(ParishId, CultureInfo.InvariantCulture, out int parsedParishId))
        {
            query = query.Where(j => j.ParishId == parsedParishId);
        }
        if (MaintenanceTeamId is not null && int.TryParse(MaintenanceTeamId, CultureInfo.InvariantCulture, out int parsedMaintenanceTeamId))
        {
            query = query.Where(j => j.MaintenanceTeamId == parsedMaintenanceTeamId);
        }
        if (OperationalStatusId is not null && int.TryParse(OperationalStatusId, CultureInfo.InvariantCulture, out int parsedOperationalStatusId))
        {
            query = query.Where(j => j.OperationalStatusId == parsedOperationalStatusId);
        }
        if (RouteTypeId is not null && int.TryParse(RouteTypeId, CultureInfo.InvariantCulture, out int parsedRouteTypeId))
        {
            query = query.Where(j => j.RouteTypeId == parsedRouteTypeId);
        }
        // Get total count before applying skip/take
        var totalCount = await query.CountAsync(cancellationToken: ct);

        query = ApplyOrdering(query, OrderBy, OrderDirection);

        var results = await query
                          .Skip(skip)
                          .Take(take)
                          .ToListAsync(cancellationToken: ct);

        return new PagedResult<Route>
        {
            TotalResults = totalCount,
            PageNumber = PageNumber,
            PageSize = take,
            Results = results
        };
    }
    private static IQueryable<Route> ApplyOrdering(IQueryable<Route> query, string orderBy, ListSortDirection orderDirection)
    {
        // Default fallback ordering
        if (string.IsNullOrWhiteSpace(orderBy) || !SortExpressions.ContainsKey(orderBy))
        {
            return query.OrderByDescending(l => l.RouteCode);
        }

        var sortExpression = SortExpressions[orderBy];

        return orderDirection == ListSortDirection.Descending
            ? query.OrderByDescending(sortExpression).ThenByDescending(l => l.RouteCode)
            : query.OrderBy(sortExpression).ThenBy(l => l.RouteCode);
    }
    public async Task<IReadOnlyCollection<LegalStatus>> GetLegalStatusOptions(CancellationToken ct = default)
    {
        //TODO: cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.RouteLegalStatuses
            .AsNoTracking()
            .OrderBy(ls => ls.Id)
            .ToArrayAsync(cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<RouteType>> GetRouteTypeOptions(CancellationToken ct = default)
    {
        //TODO: cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.RouteTypes
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .ToArrayAsync(cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<OperationalStatus>> GetOperationalStatusOptions(CancellationToken ct = default)
    {
        //TODO: cache this
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.RouteOperationalStatuses
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .ToArrayAsync(cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<string?> GetNextAvailableRouteCodeForParish(string parishCode, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var routes = await context.Routes
            .Where(r => r.RouteCode.StartsWith($"{parishCode}/"))
            .Select(r => r.RouteCode)
            .ToArrayAsync(ct)
            .ConfigureAwait(false);
        //extract the highest number from the route code (e.g. 'W1/13' would return 13)
        int highestNumber = 0;
        if (routes is not null && routes.Length != 0)
        {

            highestNumber = routes
                .Select(r => int.Parse(r.Replace(parishCode, "", StringComparison.OrdinalIgnoreCase)
                .Replace("/", "", StringComparison.OrdinalIgnoreCase), CultureInfo.InvariantCulture))
                .Max();
        }
        return $"{parishCode}/{highestNumber + 1}";
    }

    public async Task<IReadOnlyCollection<ClosedRoutesViewModel>> GetClosedRoutes(CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);

        // Get closed routes that are due to reopen within a week
        var today = LocalDate.FromDateTime(DateTime.UtcNow.Date);
        var cutoff = today.PlusDays(7);

        return await context.Routes
            .Where(r => r.ClosureIsIndefinite == false)
            .Where(r => r.ClosureEndDate != null)
            .Where(r => r.ClosureEndDate < cutoff)
            .Select(r => new ClosedRoutesViewModel
            {
                RouteCode = r.RouteCode,
                ClosureEndDate = r.ClosureEndDate.Value.ToDateOnly(),
            })
            .ToArrayAsync(cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<ClosedRoutesViewModel>> GetClosedRoutesForTeam(List<int> teamId, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);

        //Get closed routes that are due to reopen within a week
        var today = LocalDate.FromDateTime(DateTime.UtcNow.Date);
        var cutoff = today.PlusDays(7);

        return await context.Routes
            .Where(r => r.MaintenanceTeamId != null && teamId.Contains(r.MaintenanceTeamId.Value))
            .Where(r => r.ClosureIsIndefinite == false)
            .Where(r => r.ClosureEndDate != null)
            .Where(r => r.ClosureEndDate < cutoff)
            .Select(r => new ClosedRoutesViewModel
            {
                RouteCode = r.RouteCode,
                ClosureEndDate = r.ClosureEndDate!.Value.ToDateOnly(),
            })
            .ToArrayAsync(cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<bool> RouteExists(string RouteCode, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        return await context.Routes
            .AnyAsync(r => r.RouteCode == RouteCode.ToUpper(), cancellationToken: ct)
            .ConfigureAwait(false);
    }

    public async Task<Route> CreateRoute(Route route, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Routes.Add(route);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return route;
    }
    public async Task<Statement> AddStatement(Statement statement, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Statements.Add(statement);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return statement;
    }
    public async Task<Route> AddMediaToRoute(Route route, List<Media> UploadedMedia, bool IsClosureNotificationDocument, CancellationToken ct = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        context.Attach(route);
        foreach (Media media in UploadedMedia)
        {
            route.RouteMedia.Add(new RouteMedia
            {
                RouteId = route.RouteCode,
                IsClosureNotificationDocument = IsClosureNotificationDocument,
                Media = media,
            });
        }
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
        return route;

    }

    public async Task<Route> UpdateRoute(Route route, CancellationToken ct = default)
    {
        //get the existing job to enable the smarter change tracker.
        //Without this, all properties are identified as tracked, since
        //the DbContext is different from when the entity was queried
        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var existingRoute = await context.Routes.FindAsync([route.RouteCode], ct)
            ?? throw new Exception($"Route being edited (ID: {route.RouteCode}) was not found prior to updating");
        // Store the original version for concurrency checking
        uint originalVersion = route.Version;
        // Update values
        context.Entry(existingRoute).CurrentValues.SetValues(route);
        // Restore original version to ensure concurrency check works
        context.Entry(existingRoute).Property(j => j.Version).OriginalValue = originalVersion;

        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return route;
    }
}
