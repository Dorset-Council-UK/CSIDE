using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.LandownerDeposits
{
    public class LandownerDepositPublicViewModel() : LandownerDepositSimplePublicViewModel
    {
        public required MultiPolygon Geom { get; set; }

        // one to many relationships
        public ICollection<MediaPublicViewModel> Media { get; set; } = [];
        public ICollection<ContactPublicViewModel> Contacts { get; set; } = [];
        public ICollection<string> AffectedAddresses { get; set; } = [];
    }
}
