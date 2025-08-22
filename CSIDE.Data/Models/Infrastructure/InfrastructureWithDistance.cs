namespace CSIDE.Data.Models.Infrastructure
{
    public class InfrastructureWithDistance()
    {
        public required InfrastructureItem Infrastructure { get; init; }
        public double Distance { get; init; } 
        public bool AlreadyLinked { get; set; }
    }
}
