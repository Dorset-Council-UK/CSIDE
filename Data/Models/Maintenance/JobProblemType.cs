namespace CSIDE.Data.Models.Maintenance
{
    public class JobProblemType
    {
        public Job? Job { get; set; }
        public ProblemType? ProblemType { get; set; }
        public int JobId { get; set; }
        public int ProblemTypeId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is JobProblemType other)
            {
                return this.JobId == other.JobId && this.ProblemTypeId == other.ProblemTypeId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(JobId, ProblemTypeId);
        }
    }
}
