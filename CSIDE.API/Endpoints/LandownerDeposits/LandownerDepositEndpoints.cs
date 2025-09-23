using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CSIDE.API.Endpoints.LandownerDeposits;

internal static class LandownerDepositEndpoints
{
    internal static async Task<Results<Ok<ICollection<LandownerDepositSimplePublicViewModel>>, BadRequest>> GetAllLandownerDeposits(ILandownerDepositService service, CancellationToken ct)
    {
        var landownerDeposits = await service.GetAllPublicLandownerDeposits(ct).ConfigureAwait(false);
        return TypedResults.Ok(landownerDeposits);
    }

    internal static async Task<Results<Ok<LandownerDepositPublicViewModel>, NotFound>> GetLandownerDepositById(ILandownerDepositService service, int Id, int SecondaryId, CancellationToken ct)
    {
        var landownerDeposit = await service.GetPublicLandownerDepositById(Id, SecondaryId, ct).ConfigureAwait(false);
        return landownerDeposit is null ? TypedResults.NotFound() : TypedResults.Ok(landownerDeposit);
    }

    internal static async Task<Results<Ok<ICollection<LandownerDepositSimplePublicViewModel>>, NotFound>> GetLandownerDepositsBySearchParameters(
            ILandownerDepositService service,
            string[]? ParishIds,
            string? ParishId,
            string? Location,
            int MaxResults = 1000,
            CancellationToken ct = default)
    {
        var landownerDeposits = await service.GetPublicLandownerDepositsBySearchParameters(ParishIds, ParishId, Location, MaxResults, ct)
            .ConfigureAwait(false);
        return landownerDeposits is null ? TypedResults.NotFound() : TypedResults.Ok(landownerDeposits);
    }
}
