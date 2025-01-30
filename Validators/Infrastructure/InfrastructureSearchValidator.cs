using FluentValidation;
using CSIDE.Data.Models.Infrastructure;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.Infrastructure
{
    public class InfrastructureSearchValidator : AbstractValidator<InfrastructureSearch>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public InfrastructureSearchValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s).Must(s =>
                !string.IsNullOrEmpty(s.RouteID) ||
                !string.IsNullOrEmpty(s.ParishId) ||
                !string.IsNullOrEmpty(s.MaintenanceTeamId) ||
                s.ParishIds?.Length != 0 ||
                s.InfrastructureTypeId.HasValue ||
                s.InstallationDateFrom.HasValue ||
                s.InstallationDateTo.HasValue)
                .WithMessage(_localizer["Search Validation At Least One Message"]);

            RuleFor(s => s.InstallationDateFrom).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).When(s => s.InstallationDateFrom.HasValue).WithMessage(_localizer["Search Validation Date Not In Future Message",localizer["Installation Date From Label"]]);
            RuleFor(s => s.InstallationDateTo).GreaterThanOrEqualTo((s) => s.InstallationDateFrom).When(s => s.InstallationDateFrom.HasValue && s.InstallationDateTo.HasValue);
        }

    }
}