using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.DMMO
{
    public class Application
    {
        public int Id { get; set; }

        public LocalDate? ApplicationDate { get; set; }
        public LocalDate? ReceivedDate { get; set; }
        public bool IsPublic { get; set; }
        public required string ApplicationDetails { get; set; }
        public string? LocationDescription { get; set; }
        public string? PrimaryContact { get; set; }
        public string? PrimaryContactUserId { get; set; }
        public string? CaseOfficer { get; set; }
        public string? CaseOfficerUserId { get; set; }
        public string? PrivateComments { get; set; }
        public string? PublicComments { get; set; }
        public required MultiLineString Geom { get; set; }

        //linked properties
        public int ApplicationTypeId { get; set; }
        public int CaseStatusId { get; set; }
        public int ClaimedStatusId { get; set; }

        //navigation properties
        public ApplicationType? ApplicationType { get; set; }
        public ApplicationCaseStatus? CaseStatus { get; set; }
        public ApplicationClaimedStatus? ClaimedStatus { get; set; }

        //one-many relationships
        public ICollection<DMMOContact> DMMOContacts { get; } = [];
        public ICollection<DMMOAddress> DMMOAddresses { get; } = [];
        public ICollection<DMMOParish> DMMOParishes { get; } = [];
        public ICollection<DMMOMedia> DMMOMedia { get; } = [];
        public ICollection<DMMOLinkedRoute> DMMOLinkedRoutes { get; } = [];
        //Concurrency token
        public uint Version { get; set; }

    }
}
