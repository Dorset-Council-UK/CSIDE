using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.Infrastructure
{
    public class InfrastructureMedia
    {
        public int InfrastructureItemId { get; set; }
        public int MediaId { get; set; }
        public InfrastructureItem InfrastructureItem { get; set; } = null!;
        public Media Media { get; set; } = null!;
    }
}
