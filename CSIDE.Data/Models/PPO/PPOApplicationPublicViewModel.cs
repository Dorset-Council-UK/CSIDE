using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.PPO
{
    public class PPOApplicationPublicViewModel() : PPOApplicationSimplePublicViewModel
    {

        public required MultiLineString Geom { get; set; }
        //one-many relationships
        public ICollection<ContactPublicViewModel> Contacts { get; set; } = [];
        public ICollection<MediaPublicViewModel> Media { get; set; } = [];
        public ICollection<string> ApplicationTypes { get; set; } = [];
        public ICollection<OrderPublicViewModel> Orders { get; set; } = [];
    }
}
