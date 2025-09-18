namespace CSIDE.Data.Models.DMMO
{
    public class DMMOLinkedRoute
    {
        public int DMMOApplicationId { get; set; }
        public required string RouteId { get; set; }
        public DMMOApplication DMMOApplication { get; set; } = null!;
        public RightsOfWay.Route Route { get; set; } = null!;
    }
}
