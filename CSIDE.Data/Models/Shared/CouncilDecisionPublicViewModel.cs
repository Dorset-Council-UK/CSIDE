using CSIDE.Data.Models.DMMO;
using NodaTime;

namespace CSIDE.Data.Models.Shared
{
    public class CouncilDecisionPublicViewModel
    {
        public required string ReferenceNo { get; set; }
        public string? CouncilDecision { get; set; }
        public DateOnly? Date { get; set; }
        public string? Notes { get; set; }
    }
}
