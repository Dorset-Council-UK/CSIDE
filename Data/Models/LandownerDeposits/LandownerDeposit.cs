using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDeposit
    {
        public int Id { get; set; }
        public LocalDate? ReceivedDate { get; set; }
        public string? Location { get; set; }
        public LocalDate? WebsiteNoticePublished { get; set; }
        public LocalDate? EmailNoticeSent { get; set; }
        public LocalDate? OnsiteNoticeErected { get; set; }
        public string? IntendedEffect { get; set; }
        public LocalDate? ElapseDate { get; set; }
        public bool FormCompleted { get; set; }
        public bool MapCorrect { get; set; } = false;
        public bool FeePaid { get; set; } = false;
        public bool AllSigned { get; set; } = false;
        public LocalDate? DateAcknowledged {  get; set; }
        public string? ChequeReceiptNumber { get; set; }
        public LocalDate? ChequePaidInDate { get; set; }
        public LocalDate? NoticeDrafted { get; set; }
        public LocalDate? SentToArchive { get; set; }
        public string? ArchiveReference { get; set; }
        public LocalDate? WebsiteEntryAdded { get; set; }
        public string? PrimaryContact { get; set; }
        public string? PrimaryContactUserId { get; set; }
        public required MultiPolygon Geom {  get; set; }

        // one to many relationships
        public ICollection<LandownerDepositType> LandownerDepositTypes { get; } = [];
        public ICollection<LandownerDepositMedia> LandownerDepositMedia { get; } = [];
        public ICollection<LandownerDepositContact> LandownerDepositContacts { get; } = [];
        public ICollection<LandownerDepositAddress> LandownerDepositAddresses { get; } = [];
        public ICollection<LandownerDepositParish> LandownerDepositParishes { get; } = [];

        //Concurrency token
        public uint Version { get; set; }
    }
}
