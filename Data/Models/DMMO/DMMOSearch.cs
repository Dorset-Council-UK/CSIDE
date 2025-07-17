namespace CSIDE.Data.Models.DMMO
{
    public class DMMOSearch
    {
        public string[]? ParishIds { get; set; } = [];
        public string? ParishId { get; set; }
        public string? Location { get; set; }
        public DateOnly? ApplicationDateFrom { get; set; }
        public DateOnly? ApplicationDateTo { get; set; }
        public DateOnly? ReceivedDateFrom { get; set; }
        public DateOnly? ReceivedDateTo { get; set; }
        public int? ApplicationCaseStatusId { get; set; }
        public int? ApplicationTypeId { get; set; }
        public int? ApplicationClaimedStatusId { get; set; }
    }
}
