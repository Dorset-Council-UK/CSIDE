using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.Infrastructure
{
    public class InfrastructureItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public LocalDate? InstallationDate { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Length { get; set; }
        public required Point Geom { get; set; }


        public string? RouteId { get; set; }
        public int InfrastructureTypeId {  get; set; }

        public RightsOfWay.Route? Route { get; set; }
        public InfrastructureType? InfrastructureType { get; set; }

    }
}
