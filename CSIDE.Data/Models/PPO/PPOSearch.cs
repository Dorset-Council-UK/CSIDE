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
        public int? ApplicationLegislationId { get; set; }
        public int? ApplicationTypeId { get; set; }
        public int? ApplicationPriorityId { get; set; }
    }
}
