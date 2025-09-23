using CSIDE.API.Models;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class VersioningExtensions
{
    internal static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        { 
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.DefaultApiVersion = Versions.V1.Version;
        });
    }
}
