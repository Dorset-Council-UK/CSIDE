using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CSIDE.API.Endpoints.Maintenance
{
    internal static class MaintenanceJobEndpoints
    {
        internal static async Task<Results<Ok<JobPublicViewModel>, NotFound>> GetMaintenanceJobById(IMaintenanceJobsService service, int id, CancellationToken ct)
        {
            var job = await service.GetPublicMaintenanceJobById(id, ct).ConfigureAwait(false);
            return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
        }

        internal static async Task<Results<Ok<IReadOnlyCollection<JobSimplePublicViewModel>>, NotFound>> GetMaintenanceJobsBySearchParameters(
            IMaintenanceJobsService service,
            string? RouteId,
            string[]? ParishIds,
            string? ParishId,
            string? AssignedToTeamId,
            string? JobPriorityId,
            bool? IsComplete,
            string? JobStatusId,
            DateOnly? LogDateFrom,
            DateOnly? LogDateTo,
            DateOnly? CompletedDateFrom,
            DateOnly? CompletedDateTo,
            int MaxResults = 1000,
            CancellationToken ct = default)
        {
            var jobs = await service.GetPublicMaintenanceJobsBySearchParameters(RouteId, ParishIds, ParishId, AssignedToTeamId, JobPriorityId, IsComplete, JobStatusId, LogDateFrom, LogDateTo, CompletedDateFrom, CompletedDateTo, MaxResults, ct)
                .ConfigureAwait(false);
            return jobs is null ? TypedResults.NotFound() : TypedResults.Ok(jobs);
        }
    }
}
