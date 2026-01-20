using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CSIDE.API.Endpoints.RightsOfWay;

internal static class RightsOfWayEndpoints
{
    internal static async Task<Results<Ok<RouteSimpleViewModel>, NoContent, BadRequest>> GetNearestRoute(IRightsOfWayService service,
                                                                                                       ISharedDataService sharedDataService,
                                                                                                       double x,
                                                                                                       double y,
                                                                                                       int epsg = 27700,
                                                                                                       CancellationToken ct = default)
    {
        var point = new NetTopologySuite.Geometries.Point(x, y);
        if(epsg != 27700)
        {
            var transformedPoint = await sharedDataService.TransformCoordinates(x, y, epsg, 27700, ct);
            point = transformedPoint?.Geom;
        }
        if(point is null)
        {
            throw new Exception("Coordinate could not be converted to British National Grid");
        }
        point.SRID = 27700;
        var nearestRoute = await service.GetNearestRoute(point, ct: ct);
        
        if(nearestRoute is null)
        {
            return TypedResults.NoContent();
        }
        
        var routeSimpleViewModel = new RouteSimpleViewModel
        {
            RouteCode = nearestRoute.RouteCode
        };
        
        return TypedResults.Ok(routeSimpleViewModel);
    }

    
}
