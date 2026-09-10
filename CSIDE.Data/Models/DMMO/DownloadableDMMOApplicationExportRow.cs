using NodaTime;

namespace CSIDE.Data.Models.DMMO
{
    public sealed class DownloadableDMMOApplicationExportRow
    {
        public int Id { get; init; }
        public LocalDate? ApplicationDate { get; init; }
        public LocalDate? ReceivedDate { get; init; }
        public string? ApplicationDetails { get; init; }
        public string? LocationDescription { get; init; }
        public IReadOnlyCollection<string> ParishNames { get; init; } = [];
        public string? CaseStatusName { get; init; }
        public IReadOnlyCollection<string> ApplicationTypeNames { get; init; } = [];
        public IReadOnlyCollection<string> ClaimedStatusNames { get; init; } = [];
        public string? CaseOfficer { get; init; }
        public bool? Appeal { get; init; }
        public LocalDate? AppealDate { get; init; }
        public string? DirectionOfSecStateName { get; init; }
        public LocalDate? DateOfDirectionOfSecState { get; init; }
        public string? InternalArchiveReferenceNo { get; init; }
        public string? ExternalArchiveReferenceNo { get; init; }
        public string? PrivateComments { get; init; }
        public string? PublicComments { get; init; }
    }
}
