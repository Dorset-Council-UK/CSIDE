using FluentValidation;
using CSIDE.Data.Models.PPO;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.PPO
{
    public class EventValidator : AbstractValidator<PPOEvent>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public EventValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;
            RuleFor(ppoEvent => ppoEvent.EventText)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Event Label"]);

            RuleFor(ppoEvent => ppoEvent.EventDate)
                .NotEmpty()
                .WithName(_localizer["Event Date Label"]);
        }
    }
}
