using CSIDE.Data.Models.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors
{
    public class MaintenanceInterceptor(IDbContextFactory<ApplicationDbContext> contextFactory) : SaveChangesInterceptor, IMaintenanceInterceptor
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
                    await UpdateMaintenanceJobParishIds((Job)entry.Entity);
                    if (entry.State == EntityState.Added)
                    {
                        await SetMaintenanceTeamForJob((Job)entry.Entity);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Parish ID of a new or updated job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task UpdateMaintenanceJobParishIds(Job job)
        {
            using var context = contextFactory.CreateDbContext();
            var parish = await context.Parishes.SingleOrDefaultAsync(p => p.Geom.Contains(job.Geom));
            job.ParishId = parish?.ParishId;
        }

        /// <summary>
        /// Sets the Maintenance Team ID of a newly created job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task SetMaintenanceTeamForJob(Job job)
        {
            using var context = contextFactory.CreateDbContext();
            var team = await context.MaintenanceTeams.Where(t => t.Geom.Contains(job.Geom)).FirstOrDefaultAsync();
            job.MaintenanceTeamId = team?.Id;
        }

        private static bool IsCorrectEntityType(EntityEntry entry)
        {
            return entry.Entity is Job;
        }
    }
}
