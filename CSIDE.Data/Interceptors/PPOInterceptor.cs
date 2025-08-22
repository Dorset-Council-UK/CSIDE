using CSIDE.Data.Models.PPO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors
{
    public class PPOInterceptor(IDbContextFactory<ApplicationDbContext> contextFactory) : SaveChangesInterceptor, IPPOInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context;
            if (context == null) return result;

            ApplyAutomaticChanges(context).Wait();
            return result;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return result;

            await ApplyAutomaticChanges(context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task ApplyAutomaticChanges(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (IsCorrectEntityType(entry) && (entry.State is EntityState.Added or EntityState.Modified))
                {
                    if (entry.Entity is Models.PPO.Application application)
                    {
                        await UpdatePPOParishIds(application);
                    }else if(entry.Entity is PPOOrder order && entry.State is EntityState.Added)
                    {
                        await CreateOrderId(order);
                    }
                    
                }
            }
        }

        private async Task UpdatePPOParishIds(Models.PPO.Application ppoApplication)
        {
            using var context = contextFactory.CreateDbContext();
            var parishes = await context.Parishes.Where(p => p.Geom.Intersects(ppoApplication.Geom)).ToArrayAsync();
            var existingParishIds = ppoApplication.PPOParishes.Select(pp => pp.ParishId).ToList();
            var newParishIds = parishes.Select(p => p.ParishId).ToList();

            // Remove PPOParishes that are not in the new list
            var parishesToRemove = ppoApplication.PPOParishes.Where(pp => !newParishIds.Contains(pp.ParishId)).ToList();
            foreach (var parishToRemove in parishesToRemove)
            {
                ppoApplication.PPOParishes.Remove(parishToRemove);
            }

            // Add new PPOParishes that are not in the existing list
            var parishesToAdd = parishes.Where(p => !existingParishIds.Contains(p.ParishId)).ToList();
            foreach (var parishToAdd in parishesToAdd)
            {
                ppoApplication.PPOParishes.Add(new PPOParish { ParishId = parishToAdd.ParishId, ApplicationId = ppoApplication.Id });
            }
        }

        private async Task CreateOrderId(PPOOrder order)
        {
            using var context = contextFactory.CreateDbContext();
            var highestOrderNumber = await context.PPOOrders.Where(d => d.ApplicationId == order.ApplicationId)
                .OrderByDescending(d => d.OrderId)
                .Take(1)
                .Select(d => d.OrderId)
                .FirstOrDefaultAsync();
            order.OrderId = highestOrderNumber + 1;
        }

        private static bool IsCorrectEntityType(EntityEntry entry)
        {
            return entry.Entity is Models.PPO.Application or
                                   PPOOrder or
                                   PPOIntent;
        }
    }
}
