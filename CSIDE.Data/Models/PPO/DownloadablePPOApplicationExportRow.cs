using NodaTime;

namespace CSIDE.Data.Models.PPO
{
    public sealed class DownloadablePPOApplicationExportRow
    {
        public int Id { get; init; }
        public string? LegislationName { get; init; }
        public string? CaseStatusName { get; init; }
        public IReadOnlyCollection<string> ApplicationTypeNames { get; init; } = [];
        public string? ApplicationDetails { get; init; }
        public string? LocationDescription { get; init; }
        public LocalDate? ReceivedDate { get; init; }
        public string? CaseOfficer { get; init; }
        public LocalDate? DeterminationDate { get; init; }
        public bool? CouncilLandAffected { get; init; }
        public decimal? Charge { get; init; }
        public string? InternalArchiveReferenceNo { get; init; }
        public string? ExternalArchiveReferenceNo { get; init; }
        public string? PrivateComments { get; init; }
        public string? PublicComments { get; init; }
    }
}
