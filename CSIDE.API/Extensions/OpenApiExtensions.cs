using CSIDE.API.Models;
using CSIDE.Shared.Options;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class OpenApiExtensions
{
    internal static void AddOpenApiSwagger(this IServiceCollection services)
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

    internal static void MapSwaggerUI(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<CSIDEOptions>>().Value;
        var pathBase = string.IsNullOrEmpty(options.PathBase) ? "/" : $"/{options.PathBase}";

        app.UseSwaggerUI(swaggerOptions =>
        {
            swaggerOptions.InjectStylesheet($"{pathBase}css/custom.css");
            swaggerOptions.DocumentTitle = "CSIDE API";
            swaggerOptions.EnableTryItOutByDefault();
            foreach (var versionInfo in Versions.All)
            {
                swaggerOptions.SwaggerEndpoint($"{pathBase}openapi/{versionInfo.DocumentName}.json", $"CSIDE API | {versionInfo.DocumentName}");
            }
            
        });
    }
}
