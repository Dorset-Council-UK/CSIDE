using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors
{
    public class LandownerDepositInterceptor(IDbContextFactory<ApplicationDbContext> contextFactory) : SaveChangesInterceptor, ILandownerDepositInterceptor
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
                    await UpdateLandownerDepositParishIds((LandownerDeposit)entry.Entity);
                }
            }
        }

        private async Task UpdateLandownerDepositParishIds(LandownerDeposit landownerDeposit)
        {
            using var context = contextFactory.CreateDbContext();
            var parishes = await context.Parishes.Where(p => p.Geom.Intersects(landownerDeposit.Geom)).ToArrayAsync();
            landownerDeposit.LandownerDepositParishes.Clear();
            foreach (var parish in parishes)
            {
                landownerDeposit.LandownerDepositParishes.Add(new LandownerDepositParish { ParishId = parish.ParishId, LandownerDepositId = landownerDeposit.Id });
            }
        }

        private static bool IsCorrectEntityType(EntityEntry entry)
        {
            return entry.Entity is LandownerDeposit;
        }
    }
}
