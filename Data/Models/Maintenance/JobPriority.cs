namespace CSIDE.Data.Models.Maintenance
{
    public class JobPriority
    {
        public int Id {  get; set; }
        public required string Description { get; set; }
        public int SortOrder {  get; set; }
    }
}
