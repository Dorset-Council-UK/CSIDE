namespace CSIDE.Data.Models.DMMO
{
    public class DMMOLinkedRoute
    {
        public int ApplicationId { get; set; }
        public required string RouteId { get; set; }
        public Application DMMOApplication { get; set; } = null!;
        public RightsOfWay.Route Route { get; set; } = null!;
    }
}
