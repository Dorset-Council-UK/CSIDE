using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;

namespace CSIDE.API.Endpoints.DMMO;

internal static class DMMOApplicationEndpoints
{
    internal static async Task<Results<Ok<PagedResult<DMMOApplicationSimplePublicViewModel>>, BadRequest>> GetAllPublicDMMOApplications(IDMMOService service, int pageNumber = 1, int pageSize = IDMMOService.DefaultPageSize, CancellationToken ct = default)
    {
        pageSize = pageSize > IDMMOService.DefaultPageSize ? ILandownerDepositService.DefaultPageSize : pageSize;
        var applications = await service.GetAllPublicDMMOApplications(pageNumber, pageSize, ct).ConfigureAwait(false);
        return TypedResults.Ok(applications);
    }

    internal static async Task<Results<Ok<DMMOApplicationPublicViewModel>, NotFound>> GetDMMOApplicationById(IDMMOService service, int id, CancellationToken ct)
    {
        var application = await service.GetPublicDMMOApplicationById(id, ct).ConfigureAwait(false);
        return application is null ? TypedResults.NotFound() : TypedResults.Ok(application);
    }

    internal static async Task<Results<Ok<PagedResult<DMMOApplicationSimplePublicViewModel>>, NotFound>> GetDMMOApplicationsBySearchParameters(
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
            int pageNumber = 1,
            int pageSize = IDMMOService.DefaultPageSize,
            CancellationToken ct = default)
    {
        pageSize = pageSize > IDMMOService.DefaultPageSize ? ILandownerDepositService.DefaultPageSize : pageSize;
        var applications = await service.GetPublicDMMOApplicationsBySearchParameters(parishIds,
                                                                                     parishId,
                                                                                     applicationTypeId,
                                                                                     applicationCaseStatusId,
                                                                                     applicationClaimedStatusId,
                                                                                     location,
                                                                                     applicationDateFrom,
                                                                                     applicationDateTo,
                                                                                     receivedDateFrom,
                                                                                     receivedDateTo,
                                                                                     PageNumber: pageNumber,
                                                                                     PageSize: pageSize,
                                                                                     ct: ct)
            .ConfigureAwait(false);
        return applications is null ? TypedResults.NotFound() : TypedResults.Ok(applications);
    }
}
