using NodaTime;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public sealed class DownloadableLandownerDepositExportRow
    {
        public int Id { get; init; }
        public int SecondaryId { get; init; }
        public IReadOnlyCollection<string> LandownerDepositTypeNames { get; init; } = [];
        public LocalDate? ReceivedDate { get; init; }
        public string? Location { get; init; }
        public bool FormCompleted { get; init; }
        public bool MapCorrect { get; init; }
        public bool FeePaid { get; init; }
        public bool AllSigned { get; init; }
        public LocalDate? DateAcknowledged { get; init; }
        public string? ChequeReceiptNumber { get; init; }
        public LocalDate? ChequePaidInDate { get; init; }
        public LocalDate? NoticeDrafted { get; init; }
        public LocalDate? WebsiteNoticePublished { get; init; }
        public LocalDate? EmailNoticeSent { get; init; }
        public LocalDate? OnsiteNoticeErected { get; init; }
        public LocalDate? WebsiteEntryAdded { get; init; }
        public LocalDate? SentToArchive { get; init; }
        public string? InternalArchiveReferenceNo { get; init; }
        public string? ExternalArchiveReferenceNo { get; init; }
        public string? PrimaryContact { get; init; }
    }
}
