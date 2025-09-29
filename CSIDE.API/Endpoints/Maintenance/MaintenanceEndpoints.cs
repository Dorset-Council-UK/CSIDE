using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;

namespace CSIDE.API.Endpoints.Maintenance
{
    internal static class MaintenanceJobEndpoints
    {
        internal static async Task<Results<Ok<JobPublicViewModel>, NotFound>> GetMaintenanceJobById(IMaintenanceJobsService service, int id, CancellationToken ct)
        {
            var job = await service.GetPublicMaintenanceJobById(id, ct).ConfigureAwait(false);
            return job is null ? TypedResults.NotFound() : TypedResults.Ok(job);
        }

        internal static async Task<Results<Ok<PagedResult<JobSimplePublicViewModel>>, NotFound>> GetMaintenanceJobsBySearchParameters(
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
            int pageNumber = 1,
            int pageSize = IMaintenanceJobsService.DefaultPageSize,
            CancellationToken ct = default)
        {
            pageSize = pageSize > IMaintenanceJobsService.DefaultPageSize ? IMaintenanceJobsService.DefaultPageSize : pageSize;
            var jobs = await service.GetPublicMaintenanceJobsBySearchParameters(RouteId,
                                                                                ParishIds,
                                                                                ParishId,
                                                                                AssignedToTeamId,
                                                                                JobPriorityId,
                                                                                IsComplete,
                                                                                JobStatusId,
                                                                                LogDateFrom,
                                                                                LogDateTo,
                                                                                CompletedDateFrom,
                                                                                CompletedDateTo,
                                                                                PageNumber: pageNumber,
                                                                                PageSize: pageSize,
                                                                                ct: ct)
                .ConfigureAwait(false);
            return jobs is null ? TypedResults.NotFound() : TypedResults.Ok(jobs);
        }
    }
}
