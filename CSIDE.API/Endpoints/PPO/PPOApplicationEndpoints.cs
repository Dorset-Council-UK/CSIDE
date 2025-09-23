using CSIDE.Data.Models.PPO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CSIDE.API.Endpoints.PPO;

internal static class PPOApplicationEndpoints
{
    internal static async Task<Results<Ok<ICollection<PPOApplicationSimplePublicViewModel>>, BadRequest>> GetAllPPOApplications(IPPOService service, CancellationToken ct)
    {
        var applications = await service.GetAllPublicPPOApplications(ct).ConfigureAwait(false);
        return TypedResults.Ok(applications);
    }

    internal static async Task<Results<Ok<PPOApplicationPublicViewModel>, NotFound>> GetPPOApplicationById(IPPOService service, int id, CancellationToken ct)
    {
        var application = await service.GetPublicPPOApplicationById(id, ct).ConfigureAwait(false);
        return application is null ? TypedResults.NotFound() : TypedResults.Ok(application);
    }

    internal static async Task<Results<Ok<ICollection<PPOApplicationSimplePublicViewModel>>, NotFound>> GetPPOApplicationsBySearchParameters(
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
            int MaxResults = 1000,
            CancellationToken ct = default)
    {
        var applications = await service.GetPublicPPOApplicationsBySearchParameters(ParishIds, ParishId, ApplicationTypeId, ApplicationCaseStatusId, ApplicationIntentId, ApplicationPriorityId, Location, ReceivedDateFrom, ReceivedDateTo, MaxResults, ct)
            .ConfigureAwait(false);
        return applications is null ? TypedResults.NotFound() : TypedResults.Ok(applications);
    }
}
