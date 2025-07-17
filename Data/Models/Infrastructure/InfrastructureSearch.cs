namespace CSIDE.Data.Models.Infrastructure
{
    public class InfrastructureSearch
    {
        public string? RouteID { get; set; }
        public string[]? ParishIds { get; set; } = [];
        public string? ParishId { get; set; }
        public string? MaintenanceTeamId { get; set; }
        public DateOnly? InstallationDateFrom { get; set; }
        public DateOnly? InstallationDateTo { get; set; }
        public int? InfrastructureTypeId { get; set; }
    }
}
