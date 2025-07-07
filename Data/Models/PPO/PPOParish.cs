using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.PPO
{
    public class PPOParish
    {
        public int ApplicationId { get; set; }
        public int ParishId { get; set; }
        public Application PPOApplication { get; set; } = null!;
        public Parish Parish { get; set; } = null!;
    }
}
