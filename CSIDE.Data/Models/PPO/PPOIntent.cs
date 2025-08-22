namespace CSIDE.Data.Models.PPO
{
    public class PPOIntent
    {
        public int ApplicationId { get; set; }
        public int IntentId { get; set; }
        public Application Application { get; set; } = null!;
        public ApplicationIntent Intent { get; set; } = null!;
    }
}
