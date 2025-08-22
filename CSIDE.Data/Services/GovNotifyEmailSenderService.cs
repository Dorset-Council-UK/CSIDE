using CSIDE.Data.Models.Surveys;
using CSIDE.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Client;

namespace CSIDE.Data.Services;

public class GovNotifyEmailSender(ILogger<GovNotifyEmailSender> logger,
                            NotificationClient notificationClient,
                            IOptions<GovNotifySettings> options,
                            IUserService userService,
                            IDbContextFactory<ApplicationDbContext> contextFactory) : IGovNotifyEmailSender
{
    private readonly GovNotifyTemplates _templates = options.Value.Templates;

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
            using var context = contextFactory.CreateDbContext();
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
