using CSIDE.Data.Models.Maintenance;

namespace CSIDE.Data.Services
{
    public interface INotificationsService
    {
        Task SendMaintenanceJobUpdatedNotifications(int jobId, int originalStatusId, int newStatusId, CancellationToken ct = default);
        Task SendMaintenanceCommentNotification(int jobId, string comment, CancellationToken ct = default);
    }
}