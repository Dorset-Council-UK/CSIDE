using CSIDE.Data.Models.PPO;
using CSIDE.Shared.Properties;
using FluentValidation;
using Microsoft.Extensions.Localization;
using NodaTime;

namespace CSIDE.Data.Validators.PPO
{
    public class PPOValidator : AbstractValidator<PPOApplication>
    {
        private readonly IStringLocalizer<Resources> _localizer;

        public PPOValidator(IStringLocalizer<Resources> localizer)
        {
            _localizer = localizer;

            // This is validated within the GeometryValidator rulesets, but the NotEmpty check helps catch when no geometry is provided at all.
            RuleFor(app => app.Geom)
                .NotEmpty()
                .WithMessage(localizer["Invalid Geometry Validation Message"])
                .WithErrorCode("INVALID_GEOM");

            RuleFor(app => app.ApplicationDetails)
                .NotEmpty();
            RuleFor(app => app.ReceivedDate)
                .NotEmpty();
            RuleFor(app => app.ReceivedDate)
                .NotEmpty();
            RuleFor(app => app.LegislationId)
                .NotEmpty()
                .WithName(_localizer["PPO Legislation Label"]);
            RuleFor(app => app.CaseStatusId)
                .NotEmpty()
                .WithName(_localizer["Case Status Label"]);
            RuleFor(app => app.ReceivedDate)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(app => app.Charge)
                .PrecisionScale(8, 2, ignoreTrailingZeros: false);
            RuleFor(app => app.PriorityId)
                .NotEmpty()
                .WithMessage(_localizer[nameof(Resources.Priority_Must_Be_Selected)]);
        }
    }
}
