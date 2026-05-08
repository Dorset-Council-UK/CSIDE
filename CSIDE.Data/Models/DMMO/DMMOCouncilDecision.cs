using NodaTime;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOCouncilDecision
    {
        public int CouncilDecisionId { get; set; }
        public int DMMOApplicationId { get; set; }
        public int CouncilDecisionTypeId { get; set; }

        public LocalDate? Date { get; set; }
        public string? Notes { get; set; }

        public DMMOApplication DMMOApplication { get; set; } = null!;
        public CouncilDecisionType? CouncilDecisionType { get; set; } = null!;

    }
}
