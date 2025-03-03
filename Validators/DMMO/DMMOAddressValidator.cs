using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.DMMO
{
    public class DMMOAddressValidator : AbstractValidator<DMMOAddress>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        
        public DMMOAddressValidator(IDbContextFactory<ApplicationDbContext> contextFactory,IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;
            _contextFactory = contextFactory;
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
            using var context = _contextFactory.CreateDbContext();
            var DMMOAddress = await context.DMMOAddresses.FindAsync([ApplicationId, UPRN], cancellationToken: ct);
            return (DMMOAddress is null); 
        }

        private async Task<bool> DMMOApplicationExists(int ApplicationId, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var DMMOApplication = await context.DMMOApplication.FindAsync([ApplicationId], cancellationToken: ct);
            return (DMMOApplication is not null);
        }

    }
}
