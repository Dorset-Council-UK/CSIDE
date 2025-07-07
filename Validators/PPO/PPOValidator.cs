using FluentValidation;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.PPO;
using NodaTime;

namespace CSIDE.Validators.PPO
{
    public class PPOValidator : AbstractValidator<Application>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public PPOValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(app => app.ApplicationDetails)
                .NotEmpty();
            RuleFor(app => app.ReceivedDate)
                .NotEmpty();
            RuleFor(app => app.ReceivedDate)
                .NotEmpty();
            RuleFor(app => app.ApplicationTypeId)
                .NotEmpty()
                .WithName(_localizer["Application Type Label"]);
            RuleFor(app => app.CaseStatusId)
                .NotEmpty()
                .WithName(_localizer["Case Status Label"]);
            RuleFor(app => app.ReceivedDate)
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now));
            RuleFor(app => app.Charge)
                .PrecisionScale(8, 2, false);
        }
    }
}
