using NodaTime;

namespace CSIDE.Data.Models.Maintenance
{
    public sealed class DownloadableMaintenanceJobExportRow
    {
        public int Id { get; init; }
        public Instant? LogDate { get; init; }
        public string? ProblemDescription { get; init; }
        public LocalDate? CompletionDate { get; init; }
        public string? WorkDone { get; init; }
        public string? LoggedByName { get; init; }
        public string? JobPriorityDescription { get; init; }
        public double? Easting { get; init; }
        public double? Northing { get; init; }
        public string? JobStatusDescription { get; init; }
        public int? DuplicateJobId { get; init; }
        public string? RouteId { get; init; }
        public string? MaintenanceTeamName { get; init; }
        public string? ParishName { get; init; }
        public IReadOnlyCollection<string> ProblemTypeNames { get; init; } = [];
    }
}
