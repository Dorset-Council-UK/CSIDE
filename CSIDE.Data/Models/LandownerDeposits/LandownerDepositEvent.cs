using NodaTime;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositEvent
    {
        public int Id { get; set; }
        public required int LandownerDepositId { get; set; }
        public required int LandownerDepositSecondaryId { get; set; }
        public Instant CreatedAt { get; set; }
        public LocalDate EventDate { get; set; }
        public required string EventText { get; set; } = string.Empty;
        public string? AuthorId { get; set; }
        public string? AuthorName {  get; set; }

        public LandownerDeposit? LandownerDeposit { get; set; } = null;

    }
}
