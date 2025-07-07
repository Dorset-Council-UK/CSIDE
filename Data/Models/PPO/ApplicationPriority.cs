namespace CSIDE.Data.Models.PPO
{
    public class ApplicationPriority
    {
        public int Id {  get; set; }
        public required string Description { get; set; }
        public int SortOrder {  get; set; }
    }
}
