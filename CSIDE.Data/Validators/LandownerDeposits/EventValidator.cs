using FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.LandownerDeposits
{
    public class EventValidator : AbstractValidator<LandownerDepositEvent>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public EventValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;
            RuleFor(ldEvent => ldEvent.EventText)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Event Label"]);

            RuleFor(ldEvent => ldEvent.EventDate)
                .NotEmpty()
                .WithName(_localizer["Event Date Label"]);
        }
    }
}
