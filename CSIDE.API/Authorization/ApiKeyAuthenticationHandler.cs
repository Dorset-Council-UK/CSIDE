using CSIDE.Shared.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CSIDE.API.Authorization
{
    public class ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        IOptions<CSIDEOptions> csideOptions,
        ILoggerFactory loggerFactory,
        UrlEncoder urlEncoder
            ): AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, urlEncoder)
    {
        private readonly IOptions<CSIDEOptions> _apiOptions = csideOptions;
        public static readonly string SchemeName = "ApiKeyHeader";
        private static readonly string ApiKeyHeaderName = "X-API-Key";
        private static readonly string[] ApiKeyQueryNames = ["api-key", "key", "apikey"];

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? apiKey = null;

            // Try to get API key from header first
            if (Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeader))
            {
                apiKey = apiKeyHeader;
            }
            // If not in header, try query string (case-insensitive)
            else
            {
                foreach (var queryKey in Request.Query.Keys)
                {
                    if (ApiKeyQueryNames.Contains(queryKey, StringComparer.OrdinalIgnoreCase))
                    {
                        apiKey = Request.Query[queryKey];
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("API key is missing"));
            }

            if (!_apiOptions.Value.ApiKeyAuthentication.ValidApiKeys.Contains(apiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
            }

            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, SchemeName)], SchemeName
            );

            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
        }
    }
}
