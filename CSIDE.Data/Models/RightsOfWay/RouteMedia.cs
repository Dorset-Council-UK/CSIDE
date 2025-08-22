using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Models.RightsOfWay
{
    public class RouteMedia
    {
        public required string RouteId { get; set; }
        public int MediaId { get; set; }
        public bool IsClosureNotificationDocument { get; set; }
        public Route Route { get; set; } = null!;
        public Media Media { get; set; } = null!;
    }
}
