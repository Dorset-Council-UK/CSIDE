using CSIDE.Data.Models.Shared;

namespace CSIDE.Services
{
    public interface ISettingsService
    {
        Task AddRecentWork(string entityId, string entityType, string entityDescription, string url);
        Task<IList<RecentWork>> GetRecentWork();
    }
}