using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;

namespace CSIDE.API.Endpoints.PPO;

internal static class PPOApplicationEndpoints
{
    internal static async Task<Results<Ok<PagedResult<PPOApplicationSimplePublicViewModel>>, BadRequest>> GetAllPPOApplications(IPPOService service, int pageNumber = 1, int pageSize = IPPOService.DefaultPublicPageSize, CancellationToken ct = default)
    {
        var applications = await service.GetAllPublicPPOApplications(pageNumber, pageSize, ct).ConfigureAwait(false);
        return TypedResults.Ok(applications);
    }

    internal static async Task<Results<Ok<PPOApplicationPublicViewModel>, NotFound>> GetPPOApplicationById(IPPOService service, int id, CancellationToken ct)
    {
        var application = await service.GetPublicPPOApplicationById(id, ct).ConfigureAwait(false);
        return application is null ? TypedResults.NotFound() : TypedResults.Ok(application);
    }

    internal static async Task<Results<Ok<PagedResult<PPOApplicationSimplePublicViewModel>>, NotFound>> GetPPOApplicationsBySearchParameters(
            IPPOService service,
            string[]? ParishIds,
            string? ParishId,
            string? ApplicationTypeId,
            string? ApplicationCaseStatusId,
            string? ApplicationIntentId,
            string? ApplicationPriorityId,
            string? Location,
            DateOnly? ReceivedDateFrom,
            DateOnly? ReceivedDateTo,
            int pageNumber = 1,
            int pageSize = IDMMOService.DefaultPageSize,
            CancellationToken ct = default)
    {
        var applications = await service.GetPublicPPOApplicationsBySearchParameters(ParishIds,
                                                                                    ParishId,
                                                                                    ApplicationTypeId,
                                                                                    ApplicationCaseStatusId,
                                                                                    ApplicationIntentId,
                                                                                    ApplicationPriorityId,
                                                                                    Location,
                                                                                    ReceivedDateFrom,
                                                                                    ReceivedDateTo,
                                                                                    PageNumber: pageNumber,
                                                                                    PageSize: pageSize,
                                                                                    ct: ct)
            .ConfigureAwait(false);
        return applications is null ? TypedResults.NotFound() : TypedResults.Ok(applications);
    }
}
