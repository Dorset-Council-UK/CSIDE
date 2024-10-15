namespace CSIDE.Data.Models.Maintenance
{
    public class JobStatus
    {
        public int Id { get; set; }
        public required string Description { get; set; }
        public required string FriendlyDescription { get; set; }
        public bool IsComplete {  get; set; }
        public int SortOrder { get; set; }
    }
}
