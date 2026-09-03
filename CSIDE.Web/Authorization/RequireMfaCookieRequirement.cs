using Microsoft.AspNetCore.Authorization;

namespace CSIDE.Web.Authorization;

public class RequireMfaCookieRequirement : IAuthorizationRequirement
{
}

public class RequireMfaRequirementHandler : AuthorizationHandler<RequireMfaCookieRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireMfaCookieRequirement requirement)
    {
        if (MfaClaimsHelper.IsMfaAuthenticated(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
