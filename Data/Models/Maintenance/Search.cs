using NodaTime;

namespace CSIDE.Data.Models.Maintenance
{
    public class Search
    {
        public string? RouteID { get; set; }
        public string? AssignedToTeamId { get; set; }
        public string[]? ParishId { get; set; } = [];
        public DateOnly? LogDateFrom { get; set; }
        public DateOnly? LogDateTo { get; set; }
        public int? JobStatusId { get; set; }
        public int? JobPriorityId { get; set; }
        public DateOnly? CompletedDateFrom { get; set; }
        public DateOnly? CompletedDateTo { get; set; }
    }
}
