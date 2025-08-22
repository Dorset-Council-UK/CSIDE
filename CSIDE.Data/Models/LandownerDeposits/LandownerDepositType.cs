namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositType
    {
        public LandownerDeposit? LandownerDeposit { get; set; }
        public LandownerDepositTypeName? LandownerDepositTypeName { get; set; }
        public int LandownerDepositId { get; set; }
        public int LandownerDepositSecondaryId { get; set; }
        public int LandownerDepositTypeNameId { get; set; }
    }
}
