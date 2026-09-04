using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSIDE.Web.Authorization;
using CSIDE.Web.Extensions;
using System.Globalization;

namespace CSIDE.Controllers;

[Authorize]
[Route("Account/[action]")]
public class AccountController : Controller
{
    private const string DefaultManagementPath = "/management";
    private const string AccessDeniedPath = "/Account/AccessDenied";
    private const string StepUpRetryCookieName = "stepupretry";
    private const int MaxStepUpAttempts = 3;
    private static readonly string ClaimsChallenge = $"{{\"id_token\":{{\"acrs\":{{\"essential\":true,\"value\":\"{AuthenticationContextConstants.ManagementMfa}\"}}}}}}";

    /// <summary>
    /// Challenges the current signed-in user for the management MFA auth context.
    /// </summary>
    [HttpGet]
    public IActionResult BeginStepUp([FromQuery] string? returnUrl)
    {
        var localReturnUrl = string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl)
            ? DefaultManagementPath
            : returnUrl;

        var pathBase = HttpContext.Request.PathBase;
        if (pathBase.HasValue && !localReturnUrl.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase))
        {
            localReturnUrl = pathBase.Add(localReturnUrl).ToString();
        }

        if (User.HasAuthenticationContext(AuthenticationContextConstants.ManagementMfa))
        {
            Response.Cookies.Delete(StepUpRetryCookieName);
            return LocalRedirect(localReturnUrl);
        }

        var currentAttempt = 0;
        if (Request.Cookies.TryGetValue(StepUpRetryCookieName, out var attemptValue)
            && int.TryParse(attemptValue, out var parsedAttempt)
            && parsedAttempt > 0)
        {
            currentAttempt = parsedAttempt;
        }

        if (currentAttempt >= MaxStepUpAttempts)
        {
            Response.Cookies.Delete(StepUpRetryCookieName);
            return LocalRedirect(HttpContext.Request.PathBase.Add(AccessDeniedPath).ToString());
        }

        Response.Cookies.Append(
            StepUpRetryCookieName,
            (currentAttempt + 1).ToString(CultureInfo.InvariantCulture),
            new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(10),
            });

        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = localReturnUrl,
        };

        authenticationProperties.Items[AuthenticationContextConstants.StepUpAcrValuesItemKey] = AuthenticationContextConstants.ManagementMfa;
        authenticationProperties.Items[AuthenticationContextConstants.StepUpClaimsItemKey] = ClaimsChallenge;

        return Challenge(authenticationProperties, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
