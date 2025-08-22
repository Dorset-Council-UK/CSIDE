using CSIDE.Data.Models.Surveys;
using CSIDE.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CSIDE.Data.Interceptors
{
    public class SurveyInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor, ISurveyInterceptor
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
                if (IsCorrectEntityType(entry) && (entry.State is EntityState.Added))
                {
                    await SetLoggedByUserAsync((BridgeSurvey)entry.Entity);
                }
            }
        }

        /// <summary>
        /// Sets the LoggedById and LoggedByName of a newly added job to the currently logged in user
        /// </summary>
        /// <returns></returns>
        private async Task SetLoggedByUserAsync(BridgeSurvey survey)
        {
            var userId = await currentUserService.GetUserIdAsync();
            var userName = await currentUserService.GetUserNameAsync();

            survey.SurveyorId = userId;
            survey.SurveyorName = userName;
            
        }

        private static bool IsCorrectEntityType(EntityEntry entry)
        {
            return entry.Entity is BridgeSurvey;
        }
    }
}
