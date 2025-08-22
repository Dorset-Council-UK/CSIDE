namespace CSIDE.Data.Models.PPO
{
    public class ApplicationCaseStatus
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required bool IsClosed { get; set; }
    }
}
