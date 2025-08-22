using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.Validators.DMMO
{
    public class SearchValidator : AbstractValidator<DMMOSearch>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public SearchValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s).Must(s =>
                !string.IsNullOrEmpty(s.ParishId) ||
                s.ParishIds?.Length != 0 ||
                !string.IsNullOrEmpty(s.Location) ||
                s.ApplicationDateFrom.HasValue ||
                s.ApplicationDateTo.HasValue ||
                s.ReceivedDateFrom.HasValue ||
                s.ReceivedDateTo.HasValue ||
                s.ApplicationCaseStatusId.HasValue ||
                s.ApplicationClaimedStatusId.HasValue ||
                s.ApplicationTypeId.HasValue)
                .WithMessage(_localizer["Search Validation At Least One Message"]);

            RuleFor(s => s.ApplicationDateFrom).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).When(s => s.ApplicationDateFrom.HasValue).WithMessage(_localizer["Search Validation Date Not In Future Message", localizer["Log Date From Label"]]);
            RuleFor(s => s.ReceivedDateFrom).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).When(s => s.ReceivedDateFrom.HasValue).WithMessage(_localizer["Search Validation Date Not In Future Message", localizer["Completed Date From Label"]]);
            RuleFor(s => s.ApplicationDateTo).GreaterThanOrEqualTo((s) => s.ApplicationDateFrom).When(s => s.ApplicationDateFrom.HasValue && s.ApplicationDateTo.HasValue);
            RuleFor(s => s.ReceivedDateTo).GreaterThanOrEqualTo((s) => s.ReceivedDateFrom).When(s => s.ReceivedDateTo.HasValue && s.ReceivedDateTo.HasValue);


        }

    }
}