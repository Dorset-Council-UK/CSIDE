using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.Maintenance
{
    public class JobSimplePublicViewModel
    {
        public int Id { get; set; }
        public required string ReferenceNo { get; set; }
        public DateOnly? LogDate { get; set; }
        public DateOnly? CompletionDate { get; set; }
        public string? Status { get; set; }
        public bool IsComplete { get; set; }
        public bool IsDuplicate { get; set; }
        public string? Priority { get; set; }
        public string? Route { get; set; }
        public string? MaintenanceTeam { get; set; }
        public string? Parish { get; set; }

    }
}
