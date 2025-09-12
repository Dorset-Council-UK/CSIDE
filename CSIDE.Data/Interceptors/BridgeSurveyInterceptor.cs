using CSIDE.Data.Models.Surveys;
using CSIDE.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace CSIDE.Data.Interceptors;

internal class BridgeSurveyInterceptor : ISaveChangesInterceptor
{
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        => throw new InvalidOperationException("Save changes asynchronously for bridge surveys.");

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetSurveyUser(eventData.Context);
        return await ValueTask.FromResult(result);
    }

    private static void SetSurveyUser(DbContext? context)
    {
        if (context == null) return;

        var logger = context.GetService<ILogger<AuditInterceptor>>();
        var currentUserService = context.GetService<ICurrentUserService>();

        if (!currentUserService.IsAuthenticated)
        {
            logger.LogError("BridgeSurveyInterceptor - User is not authenticated.");
        }
        var userId = currentUserService.UserId;
        var userName = currentUserService.UserName;

        foreach (var entry in context.ChangeTracker.Entries<BridgeSurvey>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Entity.SurveyorId = userId;
                entry.Entity.SurveyorName = userName;
            }
        }
    }
}
