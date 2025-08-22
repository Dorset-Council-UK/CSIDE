using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.Surveys
{
    public class SurveyMedia
    {
        public int SurveyId { get; set; }
        public int MediaId { get; set; }
        public Survey Survey { get; set; } = null!;
        public Media Media { get; set; } = null!;
    }
}
