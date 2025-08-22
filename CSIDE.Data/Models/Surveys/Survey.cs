using NodaTime;

namespace CSIDE.Data.Models.Surveys
{
    public abstract class Survey
    {
        public int Id { get; set; }
        public Instant StartDate { get; set; }
        public Instant? EndDate { get; set; }
        public SurveyStatus Status { get; set; }
        public string? SurveyorId { get; set; }
        public string? SurveyorName { get; set; }
        public string? RepairsRequired { get; set; }
        public string? ValidationNotes { get; set; }
        public ICollection<SurveyMedia> SurveyMedia { get; } = [];

    }
}
