using CSIDE.Data.Models.Audit;

namespace CSIDE.Data.Services
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Add a single audit log entry.
        /// </summary>
        Task AddLogAsync(AuditLog auditLog, CancellationToken ct);

        public Task<AuditLogGridResult> GetLogsAsync(int pageNumber, int pageSize, string[]? sectionNames, string? entityId, string? userId, CancellationToken ct);
    }
}
