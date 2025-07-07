using NodaTime;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositComment
    {
        public int Id { get; set; }
        public required int LandownerDepositId { get; set; }
        public Instant CreatedAt { get; set; }
        public LocalDate CommentDate { get; set; }
        public required string CommentText { get; set; } = string.Empty;
        public string? AuthorId { get; set; }
        public string? AuthorName {  get; set; }

        public LandownerDeposit? LandownerDeposit { get; set; } = null;

    }
}
