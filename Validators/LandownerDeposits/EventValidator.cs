using FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.LandownerDeposits
{
    public class EventValidator : AbstractValidator<LandownerDepositEvent>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public EventValidator(IStringLocalizer<Properties.Resources> localizer)
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
