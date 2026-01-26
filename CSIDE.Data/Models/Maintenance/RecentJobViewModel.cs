using NodaTime;

namespace CSIDE.Data.Models.Maintenance
{
    public class RecentJobViewModel
    {
        public int Id { get; set; }
        public DateTime? LogDate { get; set; }
        public string? ProblemDescription { get; set; }
    }
}
