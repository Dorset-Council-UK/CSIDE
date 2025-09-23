using Asp.Versioning;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CSIDE.API.Models;

internal class DocumentTransformer(ApiVersion version) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var versionString = version.ToString("VVVV", ApiVersionFormatProvider.CurrentCulture);
        document.Info = new(document.Info)
        {
            Title = $"CSIDE - API | v{versionString}",
            Version = versionString,
            Description = "CSIDE API - The API for the CSIDE application.",
        };
        document.Components ??= new();
        return Task.CompletedTask;
    }
}
