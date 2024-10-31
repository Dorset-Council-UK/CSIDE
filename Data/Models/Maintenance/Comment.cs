using NodaTime;

namespace CSIDE.Data.Models.Maintenance
{
    public class Comment
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Instant CreatedAt { get; set; }
        public required string CommentText { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string? AuthorId { get; set; }
        public string? AuthorName {  get; set; }

        public Job Job { get; set; } = null!;

    }
}
