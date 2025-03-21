using CSIDE.Data.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors
{
    public class InfrastructureInterceptor(IDbContextFactory<ApplicationDbContext> contextFactory) : SaveChangesInterceptor, IInfrastructureInterceptor
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
                    await UpdateInfrastructureParishIds((InfrastructureItem)entry.Entity);
                    await SetMaintenanceTeamForInfra((InfrastructureItem)entry.Entity);
                }
            }
        }

        private async Task UpdateInfrastructureParishIds(InfrastructureItem infra)
        {
            using var context = contextFactory.CreateDbContext();
            var parish = await context.Parishes.SingleOrDefaultAsync(p => p.Geom.Contains(infra.Geom));
            infra.ParishId = parish?.ParishId;
        }

        private async Task SetMaintenanceTeamForInfra(InfrastructureItem infra)
        {
            using var context = contextFactory.CreateDbContext();
            var team = await context.MaintenanceTeams.Where(t => t.Geom.Contains(infra.Geom)).FirstOrDefaultAsync();
            infra.MaintenanceTeamId = team?.Id;
        }

        private static bool IsCorrectEntityType(EntityEntry entry)
        {
            return entry.Entity is InfrastructureItem;
        }
    }
}
