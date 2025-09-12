using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors;

internal class LandownerDepositInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for landowner deposits.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await UpdateLandownerDeposit(eventData, cancellationToken);
        return await ValueTask.FromResult(result);
    }

    private static async Task UpdateLandownerDeposit(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Context is not ApplicationDbContext context) return;

        foreach (var entry in context.ChangeTracker.Entries<LandownerDeposit>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                await AddLandownerDepositParishes(context, entry.Entity, cancellationToken);
            }
        }
    }

    private static async Task AddLandownerDepositParishes(ApplicationDbContext context, LandownerDeposit landownerDeposit, CancellationToken cancellationToken)
    {
        // Get intersecting parish IDs
        var newParishIds = await context.Parishes
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(p => p.Geom.Intersects(landownerDeposit.Geom))
            .Select(p => p.ParishId)
            .ToHashSetAsync(cancellationToken)
            .ConfigureAwait(false);

        var existingParishIds = landownerDeposit.LandownerDepositParishes
            .Select(dp => dp.ParishId)
            .ToHashSet();

        // Remove Parishes that are no longer intersecting
        var parishesToRemove = landownerDeposit.LandownerDepositParishes
            .Where(dp => !newParishIds.Contains(dp.ParishId))
            .ToList();
        foreach (var parishToRemove in parishesToRemove)
        {
            landownerDeposit.LandownerDepositParishes.Remove(parishToRemove);
        }

        // Add new Parishes that are missing
        var parishIdsToAdd = newParishIds.Except(existingParishIds);
        foreach (var parishId in parishIdsToAdd)
        {
            landownerDeposit.LandownerDepositParishes.Add(new LandownerDepositParish {
                ParishId = parishId,
                LandownerDepositId = landownerDeposit.Id
            });
        }
    }
}
