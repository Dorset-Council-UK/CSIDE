using NetTopologySuite.Geometries;

namespace CSIDE.Data.Models.Maintenance
{
    public class JobPublicViewModel : JobSimplePublicViewModel
    {
        
        public string? ProblemDescription { get; set; }
        public string? WorkDone { get; set; }
        public int? DuplicateJobId { get; set; }
        public Point? Geom { get; set; }

        //one-many relationships
        public ICollection<CommentPublicViewModel> Comments { get; set; } = [];
        public ICollection<string> ProblemTypes { get; set; } = [];
    }
}
