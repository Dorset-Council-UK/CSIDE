using CSIDE.API.Models;
using CSIDE.Shared.Options;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class OpenApiExtensions
{
    internal static void AddOpenApi(this IServiceCollection services)
    {
        foreach (var versionInfo in Versions.All)
        {
            services.AddOpenApi(versionInfo.DocumentName, options =>
            {
                options.AddDocumentTransformer(new DocumentTransformer(versionInfo.Version));
                options.AddOperationTransformer<SecurityOperationTransformer>();
            });
        }
    }
}
