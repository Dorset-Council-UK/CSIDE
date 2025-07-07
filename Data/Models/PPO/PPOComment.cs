using NodaTime;

namespace CSIDE.Data.Models.PPO
{
    public class PPOComment
    {
        public int Id { get; set; }
        public required int ApplicationId { get; set; }
        public Instant CreatedAt { get; set; }
        public LocalDate CommentDate { get; set; }
        public required string CommentText { get; set; } = string.Empty;
        public string? AuthorId { get; set; }
        public string? AuthorName {  get; set; }

        public Application? PPOApplication { get; set; } = null;

    }
}
