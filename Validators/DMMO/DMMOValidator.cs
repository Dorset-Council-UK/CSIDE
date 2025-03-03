using FluentValidation;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.DMMO;
using NodaTime;

namespace CSIDE.Validators.DMMO
{
    public class DMMOValidator : AbstractValidator<Application>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public DMMOValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(app => app.ApplicationDetails)
                .NotEmpty();
            RuleFor(app => app.ApplicationDate)
                .NotEmpty();
            RuleFor(app => app.ReceivedDate)
                .NotEmpty();
            RuleFor(app => app.ClaimedStatusId)
                .NotEmpty()
                .WithName(_localizer["Claimed Status Label"]);
            RuleFor(app => app.ApplicationTypeId)
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
