namespace CSIDE.Data.Models.RightsOfWay
{
    public class Search
    {
        public string? RouteID { get; set; }
        public string? Name { get; set; }
        public string[]? ParishIds { get; set; } = [];
        public string? ParishId { get; set; }
        public int? OperationalStatusId { get; set; }
        public int? RouteTypeId { get; set; }
        public int? MaintenanceTeamId { get; set; }
    }
}
