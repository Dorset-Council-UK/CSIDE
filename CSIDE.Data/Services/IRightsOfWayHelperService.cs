namespace CSIDE.Data.Services
{
    public interface IRightsOfWayHelperService
    {
        Task<Data.Models.RightsOfWay.Route?> GetNearestRouteAsync(NetTopologySuite.Geometries.Point location, int maxDistance = 20);
        Task<bool> RouteExistsAsync(string RouteCode);
    }
}
