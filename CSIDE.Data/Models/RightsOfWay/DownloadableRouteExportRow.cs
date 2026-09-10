using NodaTime;

namespace CSIDE.Data.Models.RightsOfWay
{
    public sealed class DownloadableRouteExportRow
    {
        public required string RouteCode { get; init; }
        public string? Name { get; init; }
        public string? RouteTypeName { get; init; }
        public string? ParishName { get; init; }
        public string? LegalStatusName { get; init; }
        public string? OperationalStatusName { get; init; }
        public bool OperationalStatusIsClosed { get; init; }
        public LocalDate? ClosureStartDate { get; init; }
        public LocalDate? ClosureEndDate { get; init; }
        public bool ClosureIsIndefinite { get; init; }
        public string? MaintenanceTeamName { get; init; }
        public string? Notes { get; init; }
        public string? LatestStatementText { get; init; }
        public string? LatestStatementStartGridRef { get; init; }
        public string? LatestStatementEndGridRef { get; init; }
    }
}
