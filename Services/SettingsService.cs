using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace CSIDE.Services;

public class SettingsService(ProtectedLocalStorage ProtectedLocalStore, ILogger<SettingsService> logger) : ISettingsService
{
    private const string RecentWorkKey = "cside.recentWork";
    public async Task<IList<RecentWork>> GetRecentWork()
    {
        try
        {
            var data = await ProtectedLocalStore.GetAsync<IList<RecentWork>>(RecentWorkKey).ConfigureAwait(false);
            if (data.Success && data.Value is not null)
            {
                return data.Value;
            }
        }catch(Exception ex)
        {
            logger.LogWarning(ex, "Recent Work localStorage item could not be fetched. Deleting storage item in case of corrupt data");
            await ProtectedLocalStore.DeleteAsync(RecentWorkKey).ConfigureAwait(false);
        }
        return [];
    }
    public async Task AddRecentWork(string entityId, string entityType, string entityDescription, string url)
    {
        IList<RecentWork> recentWork = await GetRecentWork().ConfigureAwait(false);

        // Remove any existing item with the same entityId and entityType
        var existingItem = recentWork.FirstOrDefault(w =>
            string.Equals(w.EntityId, entityId, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(w.EntityType, entityType, StringComparison.OrdinalIgnoreCase));
        if (existingItem != null)
        {
            recentWork.Remove(existingItem);
        }

        recentWork.Add(new RecentWork(entityId, entityType, entityDescription, url, DateTime.UtcNow));
        if (recentWork.Count > 5)
        {
            recentWork = [.. recentWork.TakeLast(5)];
        }

        await ProtectedLocalStore.SetAsync(RecentWorkKey, recentWork).ConfigureAwait(false);
    }
}
