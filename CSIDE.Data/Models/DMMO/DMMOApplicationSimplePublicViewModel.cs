using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOApplicationSimplePublicViewModel()
    {
        public int Id { get; set; }
        public required string ReferenceNo { get; set; }

        public DateOnly? ApplicationDate { get; set; }
        public DateOnly? ReceivedDate { get; set; }
        public required string ApplicationDetails { get; set; }
        public string? LocationDescription { get; set; }
        public string? CaseOfficer { get; set; }
        public string? PublicComments { get; set; }
        public DateOnly? DeterminationDate { get; set; }
        public bool? Appeal { get; set; }
        public DateOnly? AppealDate { get; set; }
        public DateOnly? DateOfDirectionOfSecState { get; set; }

        public string? ApplicationType { get; set; }
        public string? CaseStatus { get; set; }
        public string? ClaimedStatus { get; set; }
        public string? DirectionOfSecState { get; set; }

        public ICollection<string> Parishes { get; set; } = [];
    }
}
