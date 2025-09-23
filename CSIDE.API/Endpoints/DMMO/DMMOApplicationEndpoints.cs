using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CSIDE.API.Endpoints.DMMO;

internal static class DMMOApplicationEndpoints
{
    internal static async Task<Results<Ok<ICollection<DMMOApplicationSimplePublicViewModel>>, BadRequest>> GetAllPublicDMMOApplications(IDMMOService service, CancellationToken ct)
    {
        var applications = await service.GetAllPublicDMMOApplications(ct).ConfigureAwait(false);
        return TypedResults.Ok(applications);
    }

    internal static async Task<Results<Ok<DMMOApplicationPublicViewModel>, NotFound>> GetDMMOApplicationById(IDMMOService service, int id, CancellationToken ct)
    {
        var application = await service.GetPublicDMMOApplicationById(id, ct).ConfigureAwait(false);
        return application is null ? TypedResults.NotFound() : TypedResults.Ok(application);
    }

    internal static async Task<Results<Ok<ICollection<DMMOApplicationSimplePublicViewModel>>, NotFound>> GetDMMOApplicationsBySearchParameters(
            IDMMOService service, 
            string[]? parishIds,
            string? parishId,
            string? applicationTypeId,
            string? applicationCaseStatusId,
            string? applicationClaimedStatusId,
            string? location,
            DateOnly? applicationDateFrom,
            DateOnly? applicationDateTo,
            DateOnly? receivedDateFrom,
            DateOnly? receivedDateTo,
            int MaxResults = 1000,
            CancellationToken ct = default)
    {
        var applications = await service.GetPublicDMMOApplicationsBySearchParameters(parishIds, parishId, applicationTypeId, applicationCaseStatusId, applicationClaimedStatusId, location, applicationDateFrom, applicationDateTo, receivedDateFrom, receivedDateTo, MaxResults, ct)
            .ConfigureAwait(false);
        return applications is null ? TypedResults.NotFound() : TypedResults.Ok(applications);
    }
}
