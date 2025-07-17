namespace CSIDE.Data.Models.DMMO
{
    public class DMMOAddress
    {
        public int ApplicationId { get; set; }
        public long UPRN { get; set; }
        public string? Address { get; set; }
        public Application DMMOApplication { get; set; } = null!;
        
    }
}
