using NodaTime;

namespace CSIDE.Data.Models.Shared
{
    public class Media
    {
        public int Id { get; set; }
        public Instant? UploadDate { get; set; }
        public required string URL { get; set; }
        public string? Title { get; set; }
    }
}
