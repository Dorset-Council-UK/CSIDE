using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;

namespace CSIDE.API.Endpoints.LandownerDeposits;

internal static class LandownerDepositEndpoints
{
    internal static async Task<Results<Ok<PagedResult<LandownerDepositSimplePublicViewModel>>, BadRequest>> GetAllLandownerDeposits(ILandownerDepositService service, int pageNumber = 1, int pageSize = ILandownerDepositService.DefaultPageSize, CancellationToken ct = default)
    {
        pageSize = pageSize > IMaintenanceJobsService.DefaultPageSize ? ILandownerDepositService.DefaultPageSize : pageSize;
        var landownerDeposits = await service.GetAllPublicLandownerDeposits(pageNumber, pageSize, ct).ConfigureAwait(false);
        return TypedResults.Ok(landownerDeposits);
    }

    internal static async Task<Results<Ok<LandownerDepositPublicViewModel>, NotFound>> GetLandownerDepositById(ILandownerDepositService service, int Id, int SecondaryId, CancellationToken ct)
    {
        var landownerDeposit = await service.GetPublicLandownerDepositById(Id, SecondaryId, ct).ConfigureAwait(false);
        return landownerDeposit is null ? TypedResults.NotFound() : TypedResults.Ok(landownerDeposit);
    }

    internal static async Task<Results<Ok<PagedResult<LandownerDepositSimplePublicViewModel>>, NotFound>> GetLandownerDepositsBySearchParameters(
            ILandownerDepositService service,
            string[]? ParishIds,
            string? ParishId,
            string? Location,
            int pageNumber = 1, 
            int pageSize = ILandownerDepositService.DefaultPageSize,
            CancellationToken ct = default)
    {
        pageSize = pageSize > IMaintenanceJobsService.DefaultPageSize ? ILandownerDepositService.DefaultPageSize : pageSize;
        var landownerDeposits = await service.GetPublicLandownerDepositsBySearchParameters(ParishIds,
                                                                                           ParishId,
                                                                                           Location,
                                                                                           pageNumber: pageNumber,
                                                                                           pageSize: pageSize,
                                                                                           ct: ct)
            .ConfigureAwait(false);
        return landownerDeposits is null ? TypedResults.NotFound() : TypedResults.Ok(landownerDeposits);
    }
}
