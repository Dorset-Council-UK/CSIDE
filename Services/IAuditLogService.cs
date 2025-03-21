using BlazorBootstrap;
using CSIDE.Data.Models.Audit;

namespace CSIDE.Services
{
    public interface IAuditLogService
    {
        public Task<AuditLogGridResult> GetLogsAsync(int pageNumber, int pageSize, string[]? sectionNames, string? entityId, CancellationToken ct);
    }
}
