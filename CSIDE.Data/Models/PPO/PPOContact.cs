using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.PPO
{
    public class PPOContact
    {
        public int ApplicationId { get; set; }
        public int ContactId { get; set; }
        public Application PPOApplication { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
    }
}
