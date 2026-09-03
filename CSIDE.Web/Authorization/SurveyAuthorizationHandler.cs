using CSIDE.Data.Models.Surveys;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CSIDE.Authorization
{
    public class SurveyAuthorizationHandler(ILogger<SurveyAuthorizationHandler> logger) :
        AuthorizationHandler<IsSurveyorRequirement, Survey>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       IsSurveyorRequirement requirement,
                                                       Survey resource)
        {
            if(!(context.User.IsInRole("Administrator") || 
                context.User.IsInRole("Ranger") ||
                context.User.IsInRole("Surveyor")))
            {
                context.Fail(new AuthorizationFailureReason(this, "User is not a surveyor"));
                logger.LogInformation("User {userName} is not a surveyor", context.User.DisplayName);
                return Task.CompletedTask;
            }
            var UserID = context.User.UserId;
            if (string.Equals(UserID, resource.SurveyorId, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            logger.LogInformation("User {userName} is not the surveyor for survey {surveyId}", context.User.DisplayName, resource.Id);
            context.Fail(new AuthorizationFailureReason(this, "User is not the surveyor for this survey"));
            return Task.CompletedTask;
        }
    }

    public class IsSurveyorRequirement : IAuthorizationRequirement { }
}
