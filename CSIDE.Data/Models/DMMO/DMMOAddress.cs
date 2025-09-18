namespace CSIDE.Data.Models.DMMO
{
    public class DMMOAddress
    {
        public int DMMOApplicationId { get; set; }
        public long UPRN { get; set; }
        public string? Address { get; set; }
        public DMMOApplication DMMOApplication { get; set; } = null!;
        
    }
}
