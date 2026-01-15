namespace CSIDE.Data.Models.PPO
{
    public class PPOApplicationType
    {
        public int PPOApplicationId { get; set; }
        public int TypeId { get; set; }
        public PPOApplication PPOApplication { get; set; } = null!;
        public ApplicationType Type { get; set; } = null!;
    }
}
