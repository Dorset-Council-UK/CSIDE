using Asp.Versioning;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CSIDE.API.Models;

internal class DocumentTransformer(ApiVersion version) : IOpenApiDocumentTransformer
{
    private static string ApiKeyHeaderName = "X-API-Key";
    private static string ApiKeyQueryName = "api-key";
    
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

        // Add API Key security schemes
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["ApiKeyHeader"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyHeaderName,
            Description = "API Key in header for accessing protected endpoints"
        };

        document.Components.SecuritySchemes["ApiKeyQuery"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Query,
            Name = ApiKeyQueryName,
            Description = "API Key in query string for accessing protected endpoints (accepts: api-key, key, or apikey - case insensitive)"
        };

        return Task.CompletedTask;
    }
}
