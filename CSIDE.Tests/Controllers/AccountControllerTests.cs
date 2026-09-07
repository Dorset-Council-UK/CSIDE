using CSIDE.Controllers;
using CSIDE.Shared.Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CSIDE.Tests.Controllers;

public class AccountControllerTests
{
    [Fact]
    public void BeginStepUp_WhenManagementStepUpDisabled_RedirectsToManagement()
    {
        // Arrange
        var controller = CreateController(new StepUpAuthenticationOptions
        {
            EnableManagementStepUp = false,
        });

        // Act
        var result = controller.BeginStepUp(returnUrl: null);

        // Assert
        var redirect = Assert.IsType<LocalRedirectResult>(result);
        Assert.Equal("/management", redirect.Url);
    }

    [Fact]
    public void BeginStepUp_WhenRetryLimitReached_RedirectsToAccessDenied()
    {
        // Arrange
        var controller = CreateController(
            new StepUpAuthenticationOptions { EnableManagementStepUp = true },
            requestCookieHeader: "stepupretry=3");

        // Act
        var result = controller.BeginStepUp(returnUrl: null);

        // Assert
        var redirect = Assert.IsType<LocalRedirectResult>(result);
        Assert.Equal("/Account/AccessDenied", redirect.Url);
    }

    [Fact]
    public void BeginStepUp_WhenEnabledAndNoContext_ReturnsChallengeWithStepUpParameters()
    {
        // Arrange
        var controller = CreateController(new StepUpAuthenticationOptions
        {
            EnableManagementStepUp = true,
        });

        // Act
        var result = controller.BeginStepUp(returnUrl: null);

        // Assert
        var challenge = Assert.IsType<ChallengeResult>(result);
        Assert.Single(challenge.AuthenticationSchemes);
        Assert.Equal(OpenIdConnectDefaults.AuthenticationScheme, challenge.AuthenticationSchemes[0]);
        Assert.Equal("/management", challenge.Properties?.RedirectUri);
        Assert.Equal("c1", challenge.Properties?.Items["stepup_acr_values"]);
        Assert.Contains("\"value\":\"c1\"", challenge.Properties?.Items["stepup_claims"]);
    }

    private static AccountController CreateController(StepUpAuthenticationOptions options, string? requestCookieHeader = null)
    {
        var controller = new AccountController(Options.Create(options));

        var context = new DefaultHttpContext();

        if (!string.IsNullOrWhiteSpace(requestCookieHeader))
        {
            context.Request.Headers.Cookie = requestCookieHeader;
        }

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim(ClaimTypes.Role, "Administrator"),
            ],
            "TestAuthentication");

        context.User = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context,
        };

        return controller;
    }
}