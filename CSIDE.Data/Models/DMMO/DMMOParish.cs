using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOParish
    {
        public int DMMOApplicationId { get; set; }
        public int ParishId { get; set; }
        public DMMOApplication DMMOApplication { get; set; } = null!;
        public Parish Parish { get; set; } = null!;
    }
}
