namespace CSIDE.Data.Models.PPO
{
    public class PPOSearch
    {
        public string[]? ParishIds { get; set; } = [];
        public string? ParishId { get; set; }
        public string? Location { get; set; }
        public DateOnly? ReceivedDateFrom { get; set; }
        public DateOnly? ReceivedDateTo { get; set; }
        public int? ApplicationCaseStatusId { get; set; }
        public int? LegislationId { get; set; }
        public int? ApplicationIntentId { get; set; }
        public int? ApplicationPriorityId { get; set; }
    }
}
