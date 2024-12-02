using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.Shared
{
    public class Parish
    {
        public required string Name { get; set; }
        public int ParishId {  get; set; }
        public required MultiPolygon Geom {  get; set; }
    }
}
