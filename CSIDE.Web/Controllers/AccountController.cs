using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CSIDE.Controllers;

/// <summary>
/// Handles sign-in and sign-out for standard and MFA OIDC schemes.
/// </summary>
[Route("account")]
public class AccountController : Controller
{
    /// <summary>
    /// Challenges the user with the default OIDC scheme.
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "AzureAd");
    }

    /// <summary>
    /// Challenges the user with the MFA OIDC scheme.
    /// </summary>
    [HttpGet("login-mfa")]
    public IActionResult LoginMfa(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "AzureAdMFA");
    }

    /// <summary>
    /// Signs out both local cookie contexts and both OIDC schemes.
    /// </summary>
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        string? authenticatedScheme = null;
        if (authResult?.Ticket?.Properties?.Items is not null)
        {
            authResult.Ticket.Properties.Items.TryGetValue(".AuthScheme", out authenticatedScheme);
        }

        if (!string.Equals(authenticatedScheme, "AzureAdMFA", StringComparison.Ordinal))
        {
            authenticatedScheme = "AzureAd";
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return SignOut(
            new AuthenticationProperties { RedirectUri = "/account/signedout" },
            authenticatedScheme);
    }
}
