using FluentValidation;
using CSIDE.Data.Models.DMMO;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.DMMO
{
    public class EventValidator : AbstractValidator<DMMOEvent>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public EventValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;
            RuleFor(dmmoEvent => dmmoEvent.EventText)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Event Label"]);

            RuleFor(dmmoEvent => dmmoEvent.EventDate)
                .NotEmpty()
                .WithName(_localizer["Event Date Label"]);
        }
    }
}
