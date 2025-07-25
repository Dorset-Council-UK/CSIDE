using NodaTime;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOEvent
    {
        public int Id { get; set; }
        public required int ApplicationId { get; set; }
        public Instant CreatedAt { get; set; }
        public LocalDate EventDate { get; set; }
        public required string EventText { get; set; } = string.Empty;
        public string? AuthorId { get; set; }
        public string? AuthorName {  get; set; }

        public Application? DMMOApplication { get; set; } = null;

    }
}
