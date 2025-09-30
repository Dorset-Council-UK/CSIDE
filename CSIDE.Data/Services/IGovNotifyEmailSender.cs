using CSIDE.Data.Models.Surveys;

namespace CSIDE.Data.Services
{
    public interface IGovNotifyEmailSender
    {
        Task SendNewBridgeSurveyNotification(BridgeSurvey survey, string validationLink);
        Task<bool> SendMaintenanceJobLoggedNotificationEmail(string email, int jobId, bool receiveUpdates);
        Task<bool> SendMaintenanceJobSignUpConfirmationEmail(string email, int jobId, Guid unsubscribeToken);
    }
}