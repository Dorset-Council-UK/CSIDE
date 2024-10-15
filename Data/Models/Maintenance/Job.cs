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
        public string? CompletionNotes { get; set; }
        public Point Geom { get; set; }

        //linked properties
        [Required]
        public int? JobStatusId { get; set; }
        [Required]
        public int? JobPriorityId { get; set; }

        //navigation properties
        public JobStatus? JobStatus { get; set; }
        public JobPriority? JobPriority { get; set; }
    }
}
