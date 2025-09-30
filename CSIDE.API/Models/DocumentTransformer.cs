using Asp.Versioning;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CSIDE.API.Models;

internal class DocumentTransformer(ApiVersion version) : IOpenApiDocumentTransformer
{
    private static string ApiKeyHeaderName = "X-API-Key";
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var versionString = version.ToString("VVVV", ApiVersionFormatProvider.CurrentCulture);
        document.Info = new(document.Info)
        {
            Title = $"CSIDE - API | v{versionString}",
            Version = versionString,
            Description = "CSIDE API - The API for the CSIDE application.",
        };

        // Initialize components if not present
        document.Components ??= new();

        // Add API Key security scheme
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyHeaderName,
            Description = "API Key for accessing protected endpoints"
        };

        return Task.CompletedTask;
    }
}
