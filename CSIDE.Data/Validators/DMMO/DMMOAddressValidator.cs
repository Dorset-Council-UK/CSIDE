using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.DMMO
{
    public class DMMOAddressValidator : AbstractValidator<DMMOAddress>
    {
        private readonly IDMMOService _dmmoService;
        private readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        
        public DMMOAddressValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer, IDMMOService dmmoService)
        {
            _localizer = localizer;
            _dmmoService = dmmoService;
            RuleFor(d => d.UPRN)
                .NotEmpty()
                .WithName(_localizer["UPRN Label"])
                .MustAsync((dmmoAddress, UPRN, ct) => UPRNNotAlreadyExists(UPRN, dmmoAddress.ApplicationId, ct))
                .WithMessage(_localizer["UPRN Already Exists Validation Message"]);
            RuleFor(d => d.ApplicationId)
                .NotEmpty()
                .WithName(_localizer["Application ID Label"])
                .MustAsync(DMMOApplicationExists)
                .WithMessage(_localizer["DMMO Not Found Error Message"]);
        }

        private async Task<bool> UPRNNotAlreadyExists(long UPRN, int ApplicationId, CancellationToken ct)
        {
            return await _dmmoService.AddressExistsOnDMMO(ApplicationId, UPRN, ct) is false;
        }

        private async Task<bool> DMMOApplicationExists(int applicationId, CancellationToken ct)
        {
            return await _dmmoService.ApplicationExists(applicationId, ct);
        }
    }
}
