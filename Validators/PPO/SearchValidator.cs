using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Validators.PPO
{
    public class SearchValidator : AbstractValidator<PPOSearch>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public SearchValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s).Must(s =>
                !string.IsNullOrEmpty(s.ParishId) ||
                s.ParishIds?.Length != 0 ||
                !string.IsNullOrEmpty(s.Location) ||
                s.ReceivedDateFrom.HasValue ||
                s.ReceivedDateTo.HasValue ||
                s.ApplicationCaseStatusId.HasValue ||
                s.ApplicationIntentId.HasValue ||
                s.ApplicationTypeId.HasValue ||
                s.ApplicationPriorityId.HasValue)
                .WithMessage(_localizer["Search Validation At Least One Message"]);

            RuleFor(s => s.ReceivedDateFrom).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).When(s => s.ReceivedDateFrom.HasValue).WithMessage(_localizer["Search Validation Date Not In Future Message", localizer["Completed Date From Label"]]);
            RuleFor(s => s.ReceivedDateTo).GreaterThanOrEqualTo((s) => s.ReceivedDateFrom).When(s => s.ReceivedDateTo.HasValue && s.ReceivedDateTo.HasValue);


        }

    }
}