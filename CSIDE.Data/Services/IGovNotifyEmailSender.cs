using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Surveys;

namespace CSIDE.Data.Services
{
    public interface IGovNotifyEmailSender
    {
        Task SendNewBridgeSurveyNotification(BridgeSurvey survey, string validationLink);
        Task<bool> SendMaintenanceJobLoggedNotificationEmail(string email, int jobId, bool receiveUpdates);
        Task<bool> SendMaintenanceJobUpdatedNotificationEmail(string email, int jobId, JobStatus jobStatus, Guid unsubscribeToken);
        Task<bool> SendMaintenanceJobCompletedNotificationEmail(string email, int jobId, string workDone, Guid unsubscribeToken);
        Task<bool> SendMaintenanceJobDuplicateNotificationEmail(string email, int jobId, int duplicateJobId, Guid unsubscribeToken);
        Task<bool> SendMaintenanceJobSignUpConfirmationEmail(string email, int jobId, Guid unsubscribeToken);
        Task<bool> SendMaintenanceCommentAddedNotificationEmail(string email, int jobId, string comment, Guid unsubscribeToken);
    }
}