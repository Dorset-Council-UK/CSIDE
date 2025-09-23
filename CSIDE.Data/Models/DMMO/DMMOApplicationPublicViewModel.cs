using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOApplicationPublicViewModel() : DMMOApplicationSimplePublicViewModel
    {

        public required MultiLineString Geom { get; set; }
        
        //one-many relationships
        public ICollection<ContactPublicViewModel> Contacts { get; set; } = [];
        public ICollection<string> AffectedAddresses { get; set; } = [];
        public ICollection<MediaPublicViewModel> Media { get; set; } = [];
        public ICollection<string> LinkedRoutes { get; set; } = [];
        public ICollection<OrderPublicViewModel> Orders { get; set; } = [];
    }
}
