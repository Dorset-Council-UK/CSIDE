using CSIDE.Data;
using CSIDE.Data.Models.LandownerDeposits;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.LandownerDeposits
{
    public class LandownerDepositAddressValidator : AbstractValidator<LandownerDepositAddress>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        
        public LandownerDepositAddressValidator(IDbContextFactory<ApplicationDbContext> contextFactory,IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;
            _contextFactory = contextFactory;
            RuleFor(d => d.UPRN)
                .NotEmpty().WithName(localizer["UPRN Label"])
                .MustAsync((landownerDepositAddress, UPRN, ct) => UPRNNotAlreadyExists(UPRN, landownerDepositAddress.LandownerDepositId, ct))
                .WithMessage(localizer["UPRN Already Exists Validation Message"]);
        }

        private async Task<bool> UPRNNotAlreadyExists(long UPRN, int LandownerDepositId, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var landownerDepositAddress = await context.LandownerDepositAddresses.FindAsync([LandownerDepositId, UPRN], cancellationToken: ct);
            return (landownerDepositAddress is null); 
        }
    }
}
