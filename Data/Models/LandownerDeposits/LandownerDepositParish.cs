using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositParish
    {
        public int LandownerDepositId { get; set; }
        public int LandownerDepositSecondaryId { get; set; }
        public int ParishId { get; set; }
        public LandownerDeposit LandownerDeposit { get; set; } = null!;
        public Parish Parish { get; set; } = null!;
    }
}
