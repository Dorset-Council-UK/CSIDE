using CSIDE.Data.Models.Maintenance;
using CSIDE.Migrations;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Data.Services;

public class NotificationsService(IDbContextFactory<ApplicationDbContext> contextFactory,
                                  IGovNotifyEmailSender govNotifyEmailSender) : INotificationsService
{
    public async Task SendMaintenanceJobUpdatedNotifications(int jobId, int originalStatusId, int newStatusId, CancellationToken ct = default)
    {
        var subscribers = await GetMaintenanceJobSubscribers(jobId, ct);

        //no subscribers, bail out
        if (subscribers.Count == 0) return;

        //job status hasn't changed, don't notify
        if (originalStatusId == newStatusId) return;

        //check the details of the statuses
        await using ApplicationDbContext? context = await contextFactory.CreateDbContextAsync(ct);

        var statuses = await context.MaintenanceJobStatuses.ToListAsync(ct);
        var originalStatus = statuses.Where(s => s.Id == originalStatusId).SingleOrDefault();
        var newStatus = statuses.Where(s => s.Id == newStatusId).SingleOrDefault();

        //job has already been 'completed', so don't notify
        if (originalStatus is not null && originalStatus.IsComplete) return;

        if(newStatus is not null)
        {
            if (newStatus.IsDuplicate)
            {
                //get the job details
                var job = await context.MaintenanceJobs.Where(j => j.Id == jobId).SingleOrDefaultAsync(ct);
                if (job?.DuplicateJobId is null) return;
                //send duplicate job notification
                foreach (var subscriber in subscribers)
                {
                    await govNotifyEmailSender.SendMaintenanceJobDuplicateNotificationEmail(subscriber.EmailAddress, jobId, job.DuplicateJobId.Value, subscriber.UnsubscribeToken);
                }
                return;
            }
            if (newStatus.IsComplete)
            {
                //get the job details
                var job = await context.MaintenanceJobs.Where(j => j.Id == jobId).SingleOrDefaultAsync(ct);
                if (job is null) return;
                //send job completed notification
                foreach (var subscriber in subscribers)
                {
                    await govNotifyEmailSender.SendMaintenanceJobCompletedNotificationEmail(subscriber.EmailAddress, jobId, job.WorkDone ?? "", subscriber.UnsubscribeToken);
                }
                return;
            }
            //send status updated notification
            foreach (var subscriber in subscribers)
            {
                await govNotifyEmailSender.SendMaintenanceJobUpdatedNotificationEmail(subscriber.EmailAddress, jobId, newStatus, subscriber.UnsubscribeToken);
            }
            return;

        }

        return;
    }

    public async Task SendMaintenanceCommentNotification(int jobId, string comment, CancellationToken ct = default)
    {
        var subscribers = await GetMaintenanceJobSubscribers(jobId, ct);
        //no subscribers, bail out
        if (subscribers.Count == 0) return;

        foreach (var subscriber in subscribers)
        {
            await govNotifyEmailSender.SendMaintenanceCommentAddedNotificationEmail(subscriber.EmailAddress, jobId, comment, subscriber.UnsubscribeToken);
        }

    }

    public async Task<ICollection<JobSubscriber>> GetMaintenanceJobSubscribers(int jobId, CancellationToken ct = default)
    {
        await using ApplicationDbContext? context = await contextFactory.CreateDbContextAsync(ct);
        var subscribers = await context.MaintenanceJobSubscribers
            .Where(s => s.JobId == jobId)
            .ToListAsync(ct);
        return subscribers;
    }

}
