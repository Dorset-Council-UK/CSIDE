using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;
using NodaTime;

namespace CSIDE.Data.Models.Maintenance
{
    public class Job
    {

        public int Id { get; set; }
        public Instant? LogDate {  get; set; }
        public string? ProblemDescription { get; set; }
        public LocalDate? CompletionDate { get; set; }
        public string? WorkDone { get; set; }
        public string? LoggedById { get; set; }
        public string? LoggedByName { get; set; }
        public int? DuplicateJobId { get; set; }
        public Point? Geom { get; set; }

        //linked properties
        public int? JobStatusId { get; set; }
        public int? JobPriorityId { get; set; }
        public string? RouteId { get; set; }
        public int? MaintenanceTeamId { get; set; }
        public int? ParishId { get; set; }

        //navigation properties
        public JobStatus? JobStatus { get; set; }
        public JobPriority? JobPriority { get; set; }
        public RoW.Route? Route { get; set; }
        public MaintenanceTeam? MaintenanceTeam { get; set; }
        public Parish? Parish { get; set; }

        //one-many relationships
        public ICollection<Comment> Comments { get; } = [];
        public ICollection<JobContact> JobContacts { get; } = [];
        public ICollection<JobMedia> JobMedia { get; } = [];
        public ICollection<JobInfrastructure> JobInfrastructure { get; } = [];
        public ICollection<JobProblemType> ProblemTypes { get; } = [];

        //Concurrency token
        public uint Version { get; set; }
    }
}
