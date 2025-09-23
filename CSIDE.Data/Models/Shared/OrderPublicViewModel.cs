using NodaTime;

namespace CSIDE.Data.Models.Shared
{
    public class OrderPublicViewModel
    {
        public required string ReferenceNo { get; set; }
        public DateOnly? ObjectionsEndDate { get; set; }
        public bool? ObjectionsReceived { get; set; }
        public bool? ObjectionsWithdrawn { get; set; }
        public int? DeterminationProcessId { get; set; }
        public string? DeterminationProcess { get; set; } = null!;
        public int? DecisionOfSecStateId { get; set; }
        public string? DecisionOfSecState { get; set; } = null!;
        public DateOnly? DateConfirmed { get; set; }
        public DateOnly? DateSealed { get; set; }
        public DateOnly? DatePublished { get; set; }

    }
}
