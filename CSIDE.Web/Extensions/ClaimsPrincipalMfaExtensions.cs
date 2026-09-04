using System.Security.Claims;

namespace CSIDE.Web.Extensions;

internal static class ClaimsPrincipalMfaExtensions
{
    private const string MultiFactorAuthenticationMethod = "mfa";

    internal static bool HasAuthenticationContext(this ClaimsPrincipal? claimsPrincipal, string authenticationContext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationContext);

        if (claimsPrincipal is null)
        {
            return false;
        }

        foreach (var claim in claimsPrincipal.FindAll("acrs"))
        {
            var values = claim.Value.Split([' ', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (values.Contains(authenticationContext, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    internal static bool HasMultiFactorAuthentication(this ClaimsPrincipal? claimsPrincipal)
    {
        if (claimsPrincipal is null)
        {
            return false;
        }

        return claimsPrincipal
            .FindAll("amr")
            .Any(claim => string.Equals(claim.Value, MultiFactorAuthenticationMethod, StringComparison.OrdinalIgnoreCase));
    }
}
