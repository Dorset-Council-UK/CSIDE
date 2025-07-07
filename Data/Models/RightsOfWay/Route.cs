using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.RightsOfWay
{
    public class Route
    {
        public required string RouteCode { get; set; }
        public string? Name { get; set; }
        public LocalDate? ClosureStartDate { get; set; }
        public LocalDate? ClosureEndDate { get; set; }
        public bool ClosureIsIndefinite { get; set; }
        public required MultiLineString Geom { get; set; }

        //linked properties
        public required int RouteTypeId { get; set; }
        public required int OperationalStatusId { get; set; }
        public required int LegalStatusId { get; set; }
        public int? MaintenanceTeamId { get; set; }
        public int? ParishId { get; set; }

        //many-many relationships
        public ICollection<RouteMedia> RouteMedia { get; } = [];
        public ICollection<Statement> Statements { get; set; } = [];

        //navigation properties
        public RouteType? RouteType { get; set; }
        public OperationalStatus? OperationalStatus { get; set; }
        public LegalStatus? LegalStatus { get; set; }
        public Maintenance.Team? MaintenanceTeam { get; set; }
        public Parish? Parish { get; set; }

        //Concurrency token
        public uint Version { get; set; }
    }
}
