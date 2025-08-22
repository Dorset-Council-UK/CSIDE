using CSIDE.Data.Models.Audit;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.Services
{
    public class AuditLogService(IDbContextFactory<ApplicationDbContext> contextFactory) : IAuditLogService
    {
        public async Task<AuditLogGridResult> GetLogsAsync(int pageNumber, int pageSize, string[]? sectionNames, string? entityId, string? userId, CancellationToken ct)
        {
            await using var context = contextFactory.CreateDbContext();
            var logs = context.AuditLogs.AsQueryable();

            if (sectionNames != null && sectionNames.Length != 0)
            {
                logs = logs.Where(l => sectionNames.Contains(l.EntityName));
            }
            if (entityId != null)
            {
                logs = logs.Where(l => l.EntityId == entityId);
            }
            if (userId != null)
            {
                logs = logs.Where(l => l.UserId == userId);
            }

            //total
            var totalResults = await logs.CountAsync(ct);

            var results = await logs
                .OrderByDescending(l => l.LogDate)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync(ct);

            return new AuditLogGridResult(results, totalResults);

        }
    }
}
