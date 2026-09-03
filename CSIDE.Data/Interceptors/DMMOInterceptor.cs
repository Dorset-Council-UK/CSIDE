using CSIDE.Data.Models.DMMO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors;

internal class DMMOInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for DMMO entities.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await UpdateDmmo(eventData, cancellationToken);
        return await ValueTask.FromResult(result);
    }

    private static async Task UpdateDmmo(DbContextEventData eventData, CancellationToken cancellationToken)
    {
        if (eventData.Context is not ApplicationDbContext context) return;

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(entry => entry.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.Entity is DMMOApplication dmmoApplication)
            {
                await UpdateDMMOParishIds(context, dmmoApplication, cancellationToken);
            }
            else if (entry.Entity is DMMOOrder dmmoOrder && entry.State is EntityState.Added)
            {
                dmmoOrder.OrderId = await NextOrderId(context, dmmoOrder.DMMOApplicationId, cancellationToken);
            }
            else if (entry.Entity is DMMOCouncilDecision dmmoCouncilDecision && entry.State is EntityState.Added)
            {
                dmmoCouncilDecision.CouncilDecisionId = await NextCouncilDecisionId(context, dmmoCouncilDecision.DMMOApplicationId, cancellationToken);
            }
        }
    }

    private static async Task UpdateDMMOParishIds(ApplicationDbContext context, DMMOApplication dmmoApplication, CancellationToken cancellationToken)
    {
        // Get intersecting parishes
        var newParishIds = await context.Parishes
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(p => p.Geom.Intersects(dmmoApplication.Geom))
            .Select(p => p.ParishId)
            .ToHashSetAsync(cancellationToken);

        var existingParishIds = dmmoApplication.DMMOParishes
            .Select(dp => dp.ParishId)
            .ToHashSet();

        // Remove DMMOParishes that are no longer intersecting
        var parishesToRemove = dmmoApplication.DMMOParishes
            .Where(dp => !newParishIds.Contains(dp.ParishId))
            .ToList();
        foreach (var parishToRemove in parishesToRemove)
        {
            dmmoApplication.DMMOParishes.Remove(parishToRemove);
        }

        // Add new DMMOParishes that are missing
        var parishIdsToAdd = newParishIds.Except(existingParishIds);
        foreach (var parishId in parishIdsToAdd)
        {
            dmmoApplication.DMMOParishes.Add(new DMMOParish { ParishId = parishId, DMMOApplicationId = dmmoApplication.Id });
        }
    }

    private static async Task<int> NextOrderId(ApplicationDbContext context, int applicationId, CancellationToken cancellationToken)
    {
        var highestOrderNumber = await context.DMMOOrders
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(d => d.DMMOApplicationId == applicationId)
            .Select(d => (int?)d.OrderId)
            .MaxAsync(cancellationToken)
            .ConfigureAwait(false) ?? 0;

        return highestOrderNumber + 1;
    }

    private static async Task<int> NextCouncilDecisionId(ApplicationDbContext context, int applicationId, CancellationToken cancellationToken)
    {
        var highestCouncilDecisionNumber = await context.DMMOCouncilDecisions
            .AsNoTracking()
            .IgnoreAutoIncludes()
            .Where(d => d.DMMOApplicationId == applicationId)
            .Select(d => (int?)d.CouncilDecisionId)
            .MaxAsync(cancellationToken)
            .ConfigureAwait(false) ?? 0;

        return highestCouncilDecisionNumber + 1;
    }
}
