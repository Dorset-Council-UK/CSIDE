namespace CSIDE.Data.Models.PPO
{
    public class PPOIntent
    {
        public int PPOApplicationId { get; set; }
        public int IntentId { get; set; }
        public PPOApplication PPOApplication { get; set; } = null!;
        public ApplicationIntent Intent { get; set; } = null!;
    }
}
