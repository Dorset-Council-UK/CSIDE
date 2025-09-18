using CSIDE.Data.Models.PPO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors;

internal class PPOInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for PPO entities.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await UpdatePpo(eventData, cancellationToken);
        return await ValueTask.FromResult(result);
    }

    private static async Task UpdatePpo(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Context is not ApplicationDbContext context) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                if (entry.Entity is PPOApplication ppoApplication)
                {
                    await UpdatePPOParishIds(context, ppoApplication, cancellationToken);
                }

                if (entry.Entity is PPOOrder ppoOrder && entry.State is EntityState.Added)
                {
                    ppoOrder.OrderId = await NextOrderId(context, ppoOrder.PPOApplicationId, cancellationToken);
                }
            }
        }
    }

    private static async Task UpdatePPOParishIds(ApplicationDbContext context, PPOApplication ppoApplication, CancellationToken cancellationToken)
    {
        // Get intersecting parishes
        var newParishIds = await context.Parishes
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(p => p.Geom.Intersects(ppoApplication.Geom))
            .Select(p => p.ParishId)
            .ToHashSetAsync(cancellationToken)
            .ConfigureAwait(false);

        var existingParishIds = ppoApplication.PPOParishes
            .Select(dp => dp.ParishId)
            .ToHashSet();

        // Remove PPOParishes that are no longer intersecting
        var parishesToRemove = ppoApplication.PPOParishes
            .Where(dp => !newParishIds.Contains(dp.ParishId))
            .ToList();
        foreach (var parishToRemove in parishesToRemove)
        {
            ppoApplication.PPOParishes.Remove(parishToRemove);
        }

        // Add new PPOParishes that are missing
        var parishIdsToAdd = newParishIds.Except(existingParishIds);
        foreach (var parishId in parishIdsToAdd)
        {
            ppoApplication.PPOParishes.Add(new PPOParish { ParishId = parishId, PPOApplicationId = ppoApplication.Id });
        }
    }

    private static async Task<int> NextOrderId(ApplicationDbContext context, int applicationId, CancellationToken cancellationToken)
    {
        var highestOrderNumber = await context.PPOOrders
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(o => o.PPOApplicationId == applicationId)
            .Select(o => (int?)o.OrderId)
            .MaxAsync(cancellationToken)
            .ConfigureAwait(false) ?? 0;

        return highestOrderNumber + 1;
    }
}
