using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.DMMO;
using NodaTime;

namespace CSIDE.Data.Validators.DMMO
{
    public class DMMOValidator : AbstractValidator<DMMOApplication>
    {
        private readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;

        public DMMOValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;

            // This is validated within the GeometryValidator rulesets, but the NotEmpty check helps catch when no geometry is provided at all.
            RuleFor(app => app.Geom)
                .NotEmpty()
                .WithMessage(localizer["Invalid Geometry Validation Message"])
                .WithErrorCode("INVALID_GEOM");

            RuleFor(app => app.ApplicationDetails)
                .NotEmpty();
            RuleFor(app => app.ApplicationDate)
                .NotEmpty();
            RuleFor(app => app.ReceivedDate)
                .NotEmpty();
            RuleFor(app => app.DMMOApplicationTypes)
                .NotEmpty()
                .WithName(_localizer["Application Type Label"]);
            RuleFor(app => app.CaseStatusId)
                .NotEmpty()
                .WithName(_localizer["Case Status Label"]);

            RuleFor(app => app.ApplicationDate)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(app => app.ReceivedDate)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
        }
    }
}
