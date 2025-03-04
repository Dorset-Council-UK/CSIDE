namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositSearch
    {
        public string[]? ParishIds { get; set; } = [];
        public string? ParishId { get; set; }
        public string? Location { get; set; }
    }
}
