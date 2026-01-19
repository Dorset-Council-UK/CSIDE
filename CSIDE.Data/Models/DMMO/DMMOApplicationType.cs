namespace CSIDE.Data.Models.DMMO
{
    public class DMMOApplicationType
    {
        public int DMMOApplicationId { get; set; }
        public int ApplicationTypeId { get; set; }
        public DMMOApplication DMMOApplication { get; set; } = null!;
        public ApplicationType ApplicationType { get; set; } = null!;
    }
}
