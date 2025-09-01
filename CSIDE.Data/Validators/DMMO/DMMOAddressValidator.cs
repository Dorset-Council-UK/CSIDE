using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.DMMO
{
    public class DMMOAddressValidator : AbstractValidator<DMMOAddress>
    {
        readonly IDMMOService _dmmoService;
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        
        public DMMOAddressValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer, IDMMOService dmmoService)
        {
            _localizer = localizer;
            _dmmoService = dmmoService;
            RuleFor(d => d.UPRN)
                .NotEmpty().WithName(_localizer["UPRN Label"])
                .MustAsync((dmmoAddress, UPRN, ct) => UPRNNotAlreadyExists(UPRN, dmmoAddress.ApplicationId, ct))
                .WithMessage(_localizer["UPRN Already Exists Validation Message"]);
            RuleFor(d => d.ApplicationId)
                .NotEmpty().WithName(_localizer["Application ID Label"])
                .MustAsync(DMMOApplicationExists).WithMessage(_localizer["DMMO Not Found Error Message"]);
        }

        private async Task<bool> UPRNNotAlreadyExists(long UPRN, int ApplicationId, CancellationToken ct)
        {
            return await _dmmoService.AddressExistsOnDMMO(ApplicationId, UPRN, ct) is false;
        }

        private async Task<bool> DMMOApplicationExists(int ApplicationId, CancellationToken ct)
        {
            var DMMOApplication = await _dmmoService.GetDMMOApplicationById(ApplicationId, ct);
            return DMMOApplication is not null;
        }

    }
}
