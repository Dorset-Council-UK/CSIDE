using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace CSIDE.Data.Interceptors
{
    public class DMMOInterceptor(IDbContextFactory<ApplicationDbContext> contextFactory) : SaveChangesInterceptor, IDMMOInterceptor
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
                    await UpdateDMMOParishIds((Models.DMMO.Application)entry.Entity);
                }
            }
        }

        private async Task UpdateDMMOParishIds(Models.DMMO.Application dmmoApplication)
        {
            using var context = contextFactory.CreateDbContext();
            var parishes = await context.Parishes.Where(p => p.Geom.Intersects(dmmoApplication.Geom)).ToArrayAsync();
            var existingParishIds = dmmoApplication.DMMOParishes.Select(dp => dp.ParishId).ToList();
            var newParishIds = parishes.Select(p => p.ParishId).ToList();

            // Remove DMMOParishes that are not in the new list
            var parishesToRemove = dmmoApplication.DMMOParishes.Where(dp => !newParishIds.Contains(dp.ParishId)).ToList();
            foreach (var parishToRemove in parishesToRemove)
            {
                dmmoApplication.DMMOParishes.Remove(parishToRemove);
            }

            // Add new DMMOParishes that are not in the existing list
            var parishesToAdd = parishes.Where(p => !existingParishIds.Contains(p.ParishId)).ToList();
            foreach (var parishToAdd in parishesToAdd)
            {
                dmmoApplication.DMMOParishes.Add(new DMMOParish { ParishId = parishToAdd.ParishId, ApplicationId = dmmoApplication.Id });
            }
        }

        private static bool IsCorrectEntityType(EntityEntry entry)
        {
            return entry.Entity is Models.DMMO.Application;
        }
    }
}
