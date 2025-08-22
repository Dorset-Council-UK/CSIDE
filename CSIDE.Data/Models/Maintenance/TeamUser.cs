namespace CSIDE.Data.Models.Maintenance
{
    public class TeamUser
    {
        public int TeamId { get; set; }
        public required string UserId { get; set; }
        public bool IsLead { get; set; }
        public Team? Team { get; set; }
    }
}
