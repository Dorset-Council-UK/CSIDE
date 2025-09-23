using CSIDE.Data;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class HealthCheckExtensions
{
    internal static void AddApiHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(tags: ["database"])
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
    }

    internal static void MapApiHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions()
        { 
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
        });

        app.MapHealthChecks("/alive", new HealthCheckOptions()
        {
            Predicate = reg => reg.Tags.Contains("live"),
        });
    }
}
