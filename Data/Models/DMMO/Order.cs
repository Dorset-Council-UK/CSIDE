using NodaTime;

namespace CSIDE.Data.Models.DMMO
{
    public class Order
    {
        public int OrderId { get; set; }
        public required int ApplicationId { get; set; }

        public LocalDate? ObjectionsEndDate { get; set; }
        public bool? ObjectionsReceived { get; set; }
        public bool? ObjectionsWithdrawn { get; set; }
        public int? DeterminationProcessId { get; set; }
        public OrderDeterminationProcess? DeterminationProcess { get; set; } = null!;
        public int? DecisionOfSecStateId { get; set; }
        public OrderDecisionOfSecState? DecisionOfSecState { get; set; } = null!;
        public LocalDate? DateConfirmed { get; set; }
        public LocalDate? DateSealed { get; set; }
        public LocalDate? DatePublished { get; set; }
        public bool? SubmitToPINS { get; set; }
    }
}
