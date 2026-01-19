using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.PPO
{
    public class PPOApplicationSimplePublicViewModel()
    {
        public int Id { get; set; }
        public required string ReferenceNo { get; set; }
        public DateOnly? ReceivedDate { get; set; }
        public bool IsPublic { get; set; }
        public required string ApplicationDetails { get; set; }
        public string? LocationDescription { get; set; }
        public string? CaseOfficer { get; set; }
        public DateOnly? DeterminationDate { get; set; }
        public bool? CouncilLandAffected { get; set; }
        public string? PublicComments { get; set; }

        public string? Legislation { get; set; }
        public string? Priority { get; set; }
        public string? CaseStatus { get; set; }

        //one-many relationships
        public ICollection<string> Parishes { get; set; } = [];
        
    }
}
