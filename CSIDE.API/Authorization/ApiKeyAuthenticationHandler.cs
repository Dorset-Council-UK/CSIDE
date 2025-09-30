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
        public static readonly string SchemeName = "ApiKey";
        private static readonly string ApiKeyHeaderName = "X-API-Key";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("API key is missing"));
            }

            if (!_apiOptions.Value.ApiKeyAuthentication.ValidApiKeys.Contains(apiKeyHeader))
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
