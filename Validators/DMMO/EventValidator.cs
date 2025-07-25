using FluentValidation;
using CSIDE.Data.Models.DMMO;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.DMMO
{
    public class EventValidator : AbstractValidator<DMMOEvent>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public EventValidator(IStringLocalizer<Properties.Resources> localizer)
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
