using Microsoft.Identity.Client;
using NodaTime;
using System.Text.Json;

namespace CSIDE.Data.Models.Audit;

public class AuditLog : IDisposable
{
    public int Id { get; set; }
    public required string EntityName { get; set; }
    public required string EntityId { get; set; }
    public string? SecondaryEntityId { get; set; }
    public required string ChangeType { get; set; }
    public required string UserName { get; set; }
    public JsonDocument? OldValues { get; set; }
    public JsonDocument? NewValues { get; set; }
    public string? UserId { get; set; }
    public Instant LogDate { get; set; }

    public void Dispose()
    {
        OldValues?.Dispose();
        NewValues?.Dispose();
        GC.SuppressFinalize(this);
    }
}
