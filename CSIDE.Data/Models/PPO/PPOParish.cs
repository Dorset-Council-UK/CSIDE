using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.PPO
{
    public class PPOParish
    {
        public int PPOApplicationId { get; set; }
        public int ParishId { get; set; }
        public PPOApplication PPOApplication { get; set; } = null!;
        public Parish Parish { get; set; } = null!;
    }
}
