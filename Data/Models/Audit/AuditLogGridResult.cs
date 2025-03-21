namespace CSIDE.Data.Models.Audit
{
    public record AuditLogGridResult(List<AuditLog> AuditLogs, int TotalCount);
}
