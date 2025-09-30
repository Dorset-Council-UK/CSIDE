using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CSIDE.API.Models;

internal class SecurityOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // Check if the endpoint requires authorization
        var authorizeData = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<IAuthorizeData>()
            .FirstOrDefault();

        if (authorizeData != null)
        {
            operation.Security =
            [
                new()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            ];
        }

        return Task.CompletedTask;
    }
}