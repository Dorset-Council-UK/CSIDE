using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.Maintenance
{
    public class MaintenanceTeam
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required Polygon Geom { get; set; }
    }
}
