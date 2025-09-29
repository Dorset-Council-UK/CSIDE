using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;
using System.ComponentModel;

namespace CSIDE.Data.Services
{
    public interface IRightsOfWayService
    {
        const int DefaultPageSize = 100;
        Task<Route?> GetRouteByCode(string routeCode, CancellationToken ct = default);
        Task<Route?> GetNearestRoute(Point location, int maxDistance = 20,CancellationToken ct = default);
        Task<ICollection<Route>> GetNearestRoutes(Geometry geometry, int maxDistance = 50, int maxRoutes = 50, CancellationToken ct = default);
        Task<ICollection<Geometry>> GetRoutesIntersecting(Polygon bboxPolygon, CancellationToken ct = default);
        Task<PagedResult<Route>> GetRoutesBySearchParameters(
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
            int PageSize = DefaultPageSize,
            CancellationToken ct = default);
        Task<IReadOnlyCollection<LegalStatus>> GetLegalStatusOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<RouteType>> GetRouteTypeOptions(CancellationToken ct = default);
        Task<IReadOnlyCollection<OperationalStatus>> GetOperationalStatusOptions(CancellationToken ct = default);
        Task<string?> GetNextAvailableRouteCodeForParish(string parishCode, CancellationToken ct = default);
        Task<Route> CreateRoute(Route route, CancellationToken ct = default);
        Task<Statement> AddStatement(Statement statement, CancellationToken ct = default);
        Task<Route> AddMediaToRoute(Route route, List<Media> UploadedMedia, bool IsClosureNotificationDocument, CancellationToken ct = default);
        Task<ICollection<Statement>> GetStatementsByRouteId(string routeId, CancellationToken ct = default);
        Task<Route> UpdateRoute(Route route, CancellationToken ct = default);
        Task<bool> RouteExists(string RouteCode, CancellationToken ct = default);
    }
}
