using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.LandownerDeposits
{
    public class LandownerDepositAddressValidator : AbstractValidator<LandownerDepositAddress>
    {
        readonly ILandownerDepositService _landownerDepositService;
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;

        public LandownerDepositAddressValidator(ILandownerDepositService landownerDepositService, IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;
            _landownerDepositService = landownerDepositService;
            RuleFor(d => d.UPRN)
                .NotEmpty().WithName(localizer["UPRN Label"])
                .MustAsync((landownerDepositAddress, UPRN, ct) =>
                    UPRNNotAlreadyExists(UPRN, landownerDepositAddress.LandownerDepositId, landownerDepositAddress.LandownerDepositSecondaryId, ct))
                .WithMessage(localizer["UPRN Already Exists Validation Message"]);
        }

        private async Task<bool> UPRNNotAlreadyExists(long UPRN, int LandownerDepositId, int LandownerDepositSecondaryId, CancellationToken ct)
        {
            var landownerDepositAddress = await _landownerDepositService.GetLandownerDepositAddressesByDepositId(LandownerDepositId, LandownerDepositSecondaryId, ct);
            return !landownerDepositAddress.Any(a => a.UPRN == UPRN);
        }
    }
}
