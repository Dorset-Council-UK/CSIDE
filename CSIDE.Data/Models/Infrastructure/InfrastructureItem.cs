using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.Infrastructure
{
    public class InfrastructureItem
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public LocalDate? InstallationDate { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Length { get; set; }
        public Point? Geom { get; set; }

        // Linked properties
        public string? RouteId { get; set; }
        public int? InfrastructureTypeId {  get; set; }
        public int? ParishId { get; set; }
        public int? MaintenanceTeamId { get; set; }

        // One-many relationships
        public ICollection<InfrastructureMedia> InfrastructureMedia { get; } = [];

        // Navigation properties
        public RightsOfWay.Route? Route { get; set; }
        public InfrastructureType? InfrastructureType { get; set; }
        public Parish? Parish { get; set; }
        public Team? MaintenanceTeam { get; set; } 
        public InfrastructureBridgeDetails? BridgeDetails { get; set; }

        //Concurrency token
        public uint Version { get; set; }
    }
}
