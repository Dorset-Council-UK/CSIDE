using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.DMMO
{
    public class DMMOParish
    {
        public int ApplicationId { get; set; }
        public int ParishId { get; set; }
        public Application DMMOApplication { get; set; } = null!;
        public Parish Parish { get; set; } = null!;
    }
}
