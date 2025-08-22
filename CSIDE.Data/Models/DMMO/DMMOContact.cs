using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOContact
    {
        public int ApplicationId { get; set; }
        public int ContactId { get; set; }
        public Application DMMOApplication { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
    }
}
