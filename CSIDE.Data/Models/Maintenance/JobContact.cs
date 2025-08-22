using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.Maintenance
{
    public class JobContact
    {
        public int JobId { get; set; }
        public int ContactId { get; set; }
        public Job Job { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
    }
}
