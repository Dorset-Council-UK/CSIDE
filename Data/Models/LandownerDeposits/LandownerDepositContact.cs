using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositContact
    {
        public int LandownerDepositId { get; set; }
        public int ContactId { get; set; }
        public LandownerDeposit LandownerDeposit { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
    }
}
