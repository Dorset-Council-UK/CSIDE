using CSIDE.Data.Models.Surveys;

namespace CSIDE.Data.Services
{
    public interface IGovNotifyEmailSender
    {
        Task SendNewBridgeSurveyNotification(BridgeSurvey survey, string validationLink);
    }
}