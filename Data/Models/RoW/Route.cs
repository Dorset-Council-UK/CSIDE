using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.RoW
{
    public class Route
    {
        public required string RouteCode {  get; set; }
        public string? Name { get; set; }
        public required MultiLineString Geom { get; set; }

    }
}
