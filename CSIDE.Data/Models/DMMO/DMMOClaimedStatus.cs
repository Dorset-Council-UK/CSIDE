namespace CSIDE.Data.Models.DMMO
{
    public class DMMOClaimedStatus
    {
        public int DMMOApplicationId { get; set; }
        public int ClaimedStatusId { get; set; }
        public DMMOApplication DMMOApplication { get; set; } = null!;
        public ApplicationClaimedStatus ClaimedStatus { get; set; } = null!;
    }
}
