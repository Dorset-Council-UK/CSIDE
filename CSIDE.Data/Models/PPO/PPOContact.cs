using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.PPO
{
    public class PPOContact
    {
        public int PPOApplicationId { get; set; }
        public int ContactId { get; set; }
        public PPOApplication PPOApplication { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
    }
}
