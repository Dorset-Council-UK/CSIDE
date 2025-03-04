namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositAddress
    {
        public int LandownerDepositId { get; set; }
        public long UPRN { get; set; }
        public string? Address { get; set; }
        public LandownerDeposit LandownerDeposit { get; set; } = null!;
    }
}
