using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.PPO
{
    public class PPOApplication
    {
        public int Id { get; set; }

        public LocalDate? ReceivedDate { get; set; }
        public bool IsPublic { get; set; }
        public required string ApplicationDetails { get; set; }
        public string? LocationDescription { get; set; }
        public string? CaseOfficer { get; set; }
        public string? CaseOfficerUserId { get; set; }
        public LocalDate? DeterminationDate { get; set; }
        public LocalDate? DateOfDirection { get; set; }
        public bool? InspectionCertification { get; set; }
        public LocalDate? InspectionCertificationDate { get; set; }
        public LocalDate? ConfirmationPublishedDate { get; set; }
        public bool? CouncilLandAffected { get; set; }
        public decimal? Charge { get; set; }
        public string? BoxNumber { get; set; }
        public string? PrivateComments { get; set; }
        public string? PublicComments { get; set; }

        public required MultiLineString Geom { get; set; }

        //linked properties
        public int ApplicationTypeId { get; set; }
        public int PriorityId { get; set; }
        public int CaseStatusId { get; set; }

        //navigation properties
        public PPOLegislation? ApplicationType { get; set; }
        public ApplicationCaseStatus? CaseStatus { get; set; }
        public ApplicationPriority? Priority { get; set; } = null!;
        //one-many relationships
        public ICollection<PPOOrder> Orders { get; } = [];
        public ICollection<PPOContact> PPOContacts { get; } = [];
        public ICollection<PPOParish> PPOParishes { get; } = [];
        public ICollection<PPOMedia> PPOMedia { get; } = [];
        public ICollection<PPOIntent> PPOIntents { get; } = [];
        public ICollection<PPOEvent> Events { get; } = [];
        //Concurrency token
        public uint Version { get; set; }
    }
}
