using NetTopologySuite.Geometries;
using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSIDE.Data.Models.Maintenance
{
    public class Job
    {

        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Instant? LogDate {  get; set; }
        [Required]
        [StringLength(4000)]
        public string? ProblemDescription { get; set; }
        public LocalDate? CompletionDate { get; set; }
        [StringLength(4000)]
        public string? WorkDone { get; set; }
        public string? LoggedById { get; set; }
        public string? LoggedByName { get; set; }
        public Point? Geom { get; set; }

        //linked properties
        [Required]
        public int? JobStatusId { get; set; }
        [Required]
        public int? JobPriorityId { get; set; }
        [Required]
        public string? RouteId { get; set; }
        public string? AssignedToTeamId { get; set; }

        //navigation properties
        public JobStatus? JobStatus { get; set; }
        public JobPriority? JobPriority { get; set; }
        public RoW.Route? Route { get; set; }
        public MaintenanceTeam? MaintenanceTeam { get; set; }
    }
}
