using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOContact
    {
        public int DMMOApplicationId { get; set; }
        public int ContactId { get; set; }
        public DMMOApplication DMMOApplication { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
    }
}
