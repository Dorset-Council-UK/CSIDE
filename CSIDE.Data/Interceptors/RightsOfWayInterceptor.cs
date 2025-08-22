using CSIDE.Data.Models.RightsOfWay;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors
{
    public class RightsOfWayInterceptor(IDbContextFactory<ApplicationDbContext> contextFactory) : SaveChangesInterceptor, IRightsOfWayInterceptor
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
                    if (entry.Entity is Models.RightsOfWay.Route route)
                    {
                        await UpdateRouteParishIds(route);
                        await SetMaintenanceTeamForRoute(route);
                        await FixClosureData(route);
                    }
                    if(entry.Entity is Statement statement)
                    {
                        await UpdateVersionOfStatement(statement);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Parish ID of a new or updated job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task UpdateRouteParishIds(Models.RightsOfWay.Route route)
        {
            using var context = contextFactory.CreateDbContext();
            var bestParish = await context.Parishes.Where(p => p.Geom.Intersects(route.Geom)).OrderByDescending(p => p.Geom.Intersection(route.Geom).Length).FirstOrDefaultAsync();
            route.ParishId = bestParish?.ParishId;
        }

        /// <summary>
        /// Sets the Maintenance Team ID of a newly created job based on a spatial contains query
        /// </summary>
        /// <returns></returns>
        private async Task SetMaintenanceTeamForRoute(Models.RightsOfWay.Route route)
        {
            using var context = contextFactory.CreateDbContext();
            var team = await context.MaintenanceTeams.Where(t => t.Geom.Contains(route.Geom)).FirstOrDefaultAsync();
            route.MaintenanceTeamId = team?.Id;
        }

        /// <summary>
        /// Makes changes to the closure data of a Right of Way based on operational status
        /// </summary>
        /// <returns></returns>
        private async Task FixClosureData(Models.RightsOfWay.Route route)
        {
            using var context = contextFactory.CreateDbContext();

            var operationalStatus = await context.RouteOperationalStatuses.FindAsync(route.OperationalStatusId);
            if (operationalStatus is not null && !operationalStatus.IsClosed)
            {
                route.ClosureStartDate = null;
                route.ClosureEndDate = null;
                route.ClosureIsIndefinite = false;
            }
            if (operationalStatus is not null && operationalStatus.IsClosed && route.ClosureIsIndefinite)
            {
                route.ClosureEndDate = null;
            }
        }

        private async Task UpdateVersionOfStatement(Statement statement)
        {
            using var context = contextFactory.CreateDbContext();

            var highestVersionNumber = await context.Statements
                .Where(s => s.RouteId == statement.RouteId)
                .Select(s => (int?)s.Version)
                .MaxAsync() ?? 0;
            statement.Version = highestVersionNumber + 1;
        }

        private static bool IsCorrectEntityType(EntityEntry entry)
        {
            return entry.Entity is Models.RightsOfWay.Route or Statement;
        }
    }
}
