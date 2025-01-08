using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.RightsOfWay;

namespace CSIDE.Validators.RightsOfWay
{
    public class SearchValidator : AbstractValidator<Search>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public SearchValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s).Must(s =>
                !string.IsNullOrEmpty(s.RouteID) ||
                !string.IsNullOrEmpty(s.Name) ||
                !string.IsNullOrEmpty(s.ParishId) ||
                s.ParishIds?.Length != 0 ||
                s.RouteTypeId.HasValue ||
                s.OperationalStatusId.HasValue ||
                s.MaintenanceTeamId.HasValue)
                .WithMessage(_localizer["Search Validation At Least One Message"]);

        }

    }
}