using CSIDE.Data.Models.Surveys;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Services
{
    public interface IGovNotifyEmailSender
    {
        Task SendNewBridgeSurveyNotification(BridgeSurvey survey, NavigationManager navigationManager);
    }
}