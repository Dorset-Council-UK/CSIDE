using NodaTime;

namespace CSIDE.Data.Models.RightsOfWay
{
    public class Statement
    {
        public int Id { get; set; }
        public required string RouteId { get; set; }
        public required string StatementText { get; set; }
        public string? StartGridRef { get; set; }
        public string? EndGridRef { get; set; }
        public int Version { get; set; }
        public Instant? CreatedDate { get; set; }

        public Route Route { get; set; } = null!;
    }
}
