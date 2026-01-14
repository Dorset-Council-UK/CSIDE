using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

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
            operation.Security ??= [];

            // Add security requirements with OR logic (either header OR query parameter)
            // Each scheme is added as a separate requirement to express OR logic
            // (multiple schemes in one requirement = AND, separate requirements = OR)
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("ApiKeyHeader", context.Document)] = []
            });
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("ApiKeyQuery", context.Document)] = []
            });
        }

        return Task.CompletedTask;
    }
}
