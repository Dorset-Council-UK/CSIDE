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
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = "/";

        returnUrl = returnUrl.StartsWith('/') ? returnUrl : "/" + returnUrl;

        var properties = new AuthenticationProperties { RedirectUri = Url.Content("~" + returnUrl) };
        properties.Items[".AuthScheme"] = "AzureAd";
        return Challenge(properties, "AzureAd");
    }

    /// <summary>
    /// Challenges the user with the MFA OIDC scheme.
    /// </summary>
    [HttpGet("login-mfa")]
    [HttpGet("loginmfa")]
    public IActionResult LoginMfa(string returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = "/";

        returnUrl = returnUrl.StartsWith('/') ? returnUrl : "/" + returnUrl;

        var properties = new AuthenticationProperties { RedirectUri = Url.Content("~" + returnUrl) };
        properties.Items[".AuthScheme"] = "AzureAdMFA";
        return Challenge(properties, "AzureAdMFA");
    }

    /// <summary>
    /// Signs out the local cookie and the OIDC scheme that was used for sign-in.
    /// </summary>
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        string? authenticatedScheme = null;
        authResult?.Ticket?.Properties?.Items?.TryGetValue(".AuthScheme", out authenticatedScheme);

        authenticatedScheme = string.Equals(authenticatedScheme, "AzureAdMFA", StringComparison.Ordinal)
            ? "AzureAdMFA"
            : "AzureAd";

        return SignOut(
            new AuthenticationProperties { RedirectUri = Url.Content("~/account/signedout") },
            CookieAuthenticationDefaults.AuthenticationScheme,
            authenticatedScheme);
    }
}
