using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositMedia
    {
        public int LandownerDepositId { get; set; }
        public int LandownerDepositSecondaryId { get; set; }
        public int MediaId { get; set; }
        public int MediaTypeId { get; set; }
        public LandownerDeposit LandownerDeposit { get; set; } = null!;
        public Media Media { get; set; } = null!;
        public LandownerDepositMediaType MediaType { get; set; } = null!;
    }
}
