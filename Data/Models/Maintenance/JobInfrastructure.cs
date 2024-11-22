using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.Maintenance
{
    public class JobInfrastructure
    {
        public int JobId { get; set; }
        public int InfrastructureId { get; set; }
        public Job Job { get; set; } = null!;
        public Infrastructure.InfrastructureItem Infrastructure { get; set; } = null!;
    }
}
