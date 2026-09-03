using System.Security.Claims;

namespace CSIDE.Web.Authorization;

public static class MfaClaimsHelper
{
    private const string CsideMfaClaimType = "cside_mfa";
    private const string CsideMfaClaimValue = "true";
    private const string AmrClaimType = "amr";
    private const string AuthMethodsReferencesClaimType = "http://schemas.microsoft.com/claims/authnmethodsreferences";
    private const string MfaClaimValue = "mfa";

    public static bool IsMfaAuthenticated(ClaimsPrincipal? user)
    {
        if (user?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        foreach (var claim in user.Claims)
        {
            if (string.Equals(claim.Type, CsideMfaClaimType, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(claim.Value, CsideMfaClaimValue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var isMfaClaimType =
                string.Equals(claim.Type, AmrClaimType, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(claim.Type, AuthMethodsReferencesClaimType, StringComparison.OrdinalIgnoreCase);

            if (isMfaClaimType && string.Equals(claim.Value, MfaClaimValue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
