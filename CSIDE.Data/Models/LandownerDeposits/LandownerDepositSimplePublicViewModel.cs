using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositSimplePublicViewModel()
    {
        public int Id { get; set; }
        public int SecondaryId { get; set; }
        public required string ReferenceNo { get; set; }
        public DateOnly? ReceivedDate { get; set; }
        public string? Location { get; set; }
        public string? PrimaryContact { get; set; }

        // one to many relationships
        public ICollection<string> DepositTypes { get; set; } = [];
        public ICollection<string> Parishes { get; set; } = [];
    }
}
