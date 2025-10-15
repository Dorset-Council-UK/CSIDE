using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Surveys;
using CSIDE.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;

namespace CSIDE.Data.Services;

public class GovNotifyEmailSender(ILogger<GovNotifyEmailSender> logger,
                            NotificationClient notificationClient,
                            IOptions<GovNotifySettings> govNotifySettings,
                            IOptions<CSIDEOptions> csideOptions,
                            IUserService userService,
                            IDbContextFactory<ApplicationDbContext> contextFactory) : IGovNotifyEmailSender
{
    private readonly GovNotifyTemplates _templates = govNotifySettings.Value.Templates;

    /// <summary>
    ///     <para>Send a confirmation link to the user via GovNotify.</para>
    ///     <para>Generating the confirmation token.</para>
    /// </summary>
    public async Task SendNewBridgeSurveyNotification(BridgeSurvey survey, string validationLink)
    {
        try
        {
            if (survey.Infrastructure is null)
            {
                logger.LogWarning("No infrastructure was found linked to survey {surveyId}", survey.Id);
                return;
            }
            //get the user who will be notified
            await using var context = await contextFactory.CreateDbContextAsync();
            var team = await context.MaintenanceTeams.Where(c => c.Geom.Contains(survey.Infrastructure.Geom)).Include(t => t.TeamUsers).FirstOrDefaultAsync();
            if (team is null)
            {
                logger.LogWarning("A team could not be found for survey {surveyId}", survey.Id);
                return;
            }
            //get team leads
            var teamLeaders = team.TeamUsers.Where(tu => tu.IsLead).ToList();
            if (teamLeaders.Count == 0)
            {
                logger.LogWarning("No team leads could be found for team {teamId}", team.Id);
                return;
            }
            // Get the validate link
            foreach (var teamLead in teamLeaders)
            {
                //get the users name and email
                var user = await userService.GetUser(teamLead.UserId);
                if (user is null || user.OtherMails is null || user.OtherMails.Count == 0)
                {
                    logger.LogWarning("Could not find user {userId} or could not find an email for them when attempting to send a bridge survey email", teamLead.UserId);
                    continue;
                }
                var personalisation = new Dictionary<string, dynamic>(StringComparer.CurrentCulture)
                {
                    { "firstName", user.DisplayName ?? "" },
                    { "bridgeRef", survey.InfrastructureItemId },
                    { "surveyorName", survey.SurveyorName ?? "Unknown user" },
                    { "validateURL", validationLink },
                };

                // Send the email
                await SendEmail(user.OtherMails[0], _templates.NewBridgeSurvey, personalisation).ConfigureAwait(false);
            }
        }catch(Exception ex)
        {
            logger.LogError(ex, "There was an error sending notification emails for bridge surveys");
        }     
    }

    /// <summary>
    /// Sends a notification email to the user who logged a maintenance job.
    /// </summary>
    /// <param name="email">The email address to send the notification to</param>
    /// <param name="jobId">The newly created maintenance job ID</param>
    /// <param name="receiveUpdates">Whether the user has chosen to sign up to receive updates</param>
    /// <returns></returns>
    public async Task<bool> SendMaintenanceJobLoggedNotificationEmail(string email, int jobId, bool receiveUpdates)
    {
        var maintRefNo = $"{csideOptions.Value.IDPrefixes.Maintenance}{jobId}";
        var publicReportUrl = $"{csideOptions.Value.PublicMaintenanceJobURL}{jobId}";
        var personalisation = new Dictionary<string, dynamic>(StringComparer.CurrentCulture)
            {
                { "problemID", maintRefNo },
                { "problemReportURL", publicReportUrl },
                { "updates", receiveUpdates ? "yes" : "no" },
            };
        // Send the email
        await SendEmail(email, _templates.NewMaintenanceJobCreated, personalisation).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Sends a confirmation email to the user who has signed up to maintenance job updates.
    /// </summary>
    /// <param name="email">The email address to send the notification to</param>
    /// <param name="jobId">The maintenance job ID</param>
    /// <param name="unsubscribeToken">The unsubscribe token used to uniquely identify this users subscription</param>
    /// <returns></returns>
    public async Task<bool> SendMaintenanceJobSignUpConfirmationEmail(string email, int jobId, Guid unsubscribeToken)
    {
        var maintRefNo = $"{csideOptions.Value.IDPrefixes.Maintenance}{jobId}";
        var publicReportUrl = $"{csideOptions.Value.PublicMaintenanceJobURL}{jobId}";
        var publicUnsubscribeUrl = $"{csideOptions.Value.PublicUnsubscribeURL}{unsubscribeToken}";
        var personalisation = new Dictionary<string, dynamic>(StringComparer.CurrentCulture)
            {
                { "problemID", maintRefNo },
                { "problemReportURL", publicReportUrl },
                { "unsubscribeURL", publicUnsubscribeUrl },
            };

        // Send the email
        await SendEmail(email, _templates.NewMaintenanceSubscription, personalisation, oneClickUnsubscribeURL: publicUnsubscribeUrl).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> SendMaintenanceJobCompletedNotificationEmail(string email, int jobId, string workDone, Guid unsubscribeToken)
    {
        var maintRefNo = $"{csideOptions.Value.IDPrefixes.Maintenance}{jobId}";
        var publicReportUrl = $"{csideOptions.Value.PublicMaintenanceJobURL}{jobId}";
        var publicUnsubscribeUrl = $"{csideOptions.Value.PublicUnsubscribeURL}{unsubscribeToken}";
        var personalisation = new Dictionary<string, dynamic>(StringComparer.CurrentCulture)
            {
                { "problemID", maintRefNo },
                { "workDone",workDone },
                { "problemReportURL", publicReportUrl },
                { "unsubscribeURL", publicUnsubscribeUrl },
            };

        // Send the email
        await SendEmail(email, _templates.MaintenanceJobCompleted, personalisation, oneClickUnsubscribeURL: publicUnsubscribeUrl).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> SendMaintenanceJobDuplicateNotificationEmail(string email, int jobId, int duplicateJobId, Guid unsubscribeToken)
    {
        var maintRefNo = $"{csideOptions.Value.IDPrefixes.Maintenance}{jobId}";
        var duplicateMaintRefNo = $"{csideOptions.Value.IDPrefixes.Maintenance}{duplicateJobId}";
        var publicReportUrl = $"{csideOptions.Value.PublicMaintenanceJobURL}{duplicateJobId}";
        var publicUnsubscribeUrl = $"{csideOptions.Value.PublicUnsubscribeURL}{unsubscribeToken}";
        var personalisation = new Dictionary<string, dynamic>(StringComparer.CurrentCulture)
            {
                { "problemID", maintRefNo },
                { "duplicateProblemID", duplicateMaintRefNo },
                { "problemReportURL", publicReportUrl },
                { "unsubscribeURL", publicUnsubscribeUrl },
            };

        // Send the email
        await SendEmail(email, _templates.MaintenanceJobDuplicate, personalisation, oneClickUnsubscribeURL: publicUnsubscribeUrl).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> SendMaintenanceJobUpdatedNotificationEmail(string email, int jobId, JobStatus newStatus, Guid unsubscribeToken)
    {
        var maintRefNo = $"{csideOptions.Value.IDPrefixes.Maintenance}{jobId}";
        var publicReportUrl = $"{csideOptions.Value.PublicMaintenanceJobURL}{jobId}";
        var publicUnsubscribeUrl = $"{csideOptions.Value.PublicUnsubscribeURL}{unsubscribeToken}";
        var personalisation = new Dictionary<string, dynamic>(StringComparer.CurrentCulture)
            {
                { "problemID", maintRefNo },
                { "newStatus", newStatus.Description },
                { "newStatusDetails",newStatus.FriendlyDescription },
                { "problemReportURL", publicReportUrl },
                { "unsubscribeURL", publicUnsubscribeUrl },
            };

        // Send the email
        await SendEmail(email, _templates.MaintenanceJobUpdated, personalisation, oneClickUnsubscribeURL: publicUnsubscribeUrl).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> SendMaintenanceCommentAddedNotificationEmail(string email, int jobId, string comment, Guid unsubscribeToken)
    {
        var maintRefNo = $"{csideOptions.Value.IDPrefixes.Maintenance}{jobId}";
        var publicReportUrl = $"{csideOptions.Value.PublicMaintenanceJobURL}{jobId}";
        var publicUnsubscribeUrl = $"{csideOptions.Value.PublicUnsubscribeURL}{unsubscribeToken}";
        var personalisation = new Dictionary<string, dynamic>(StringComparer.CurrentCulture)
            {
                { "problemID", maintRefNo },
                { "comment", comment },
                { "problemReportURL", publicReportUrl },
                { "unsubscribeURL", publicUnsubscribeUrl },
            };

        // Send the email
        await SendEmail(email, _templates.MaintenanceCommentAdded, personalisation, oneClickUnsubscribeURL: publicUnsubscribeUrl).ConfigureAwait(false);
        return true;
    }


    /// <summary>
    ///     <para>Send an email to the specified email address via GovNotify.</para>
    ///     <para>If there is a problem pass the exception back up.</para>
    /// </summary>
    /// <remarks>
    /// Error codes are documented at <see href="https://docs.notifications.service.gov.uk/net.html#send-an-email-error-codes">Error Codes</see>
    /// </remarks>
    private async Task<string> SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic>? personalisation, string? clientReference = null, string? emailReplyToId = null, string? oneClickUnsubscribeURL = null)
    {
        logger.LogDebug("Sending email to {EmailAddress}", emailAddress);

        var response = await notificationClient
            .SendEmailAsync(emailAddress, templateId, personalisation, clientReference, emailReplyToId, oneClickUnsubscribeURL)
            .ConfigureAwait(false);

        logger.LogDebug("Email sent to {EmailAddress} with GovNotify response ID {ResponseId}", emailAddress, response.id);

        return response.id;
    }
}
