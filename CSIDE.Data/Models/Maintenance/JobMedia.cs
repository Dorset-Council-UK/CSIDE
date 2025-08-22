using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.Maintenance
{
    public class JobMedia
    {
        public int JobId { get; set; }
        public int MediaId { get; set; }
        public Job Job { get; set; } = null!;
        public Media Media { get; set; } = null!;
    }
}
