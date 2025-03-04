using FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.LandownerDeposits
{
    public class LandownerDepositSearchValidator : AbstractValidator<LandownerDepositSearch>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public LandownerDepositSearchValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(s => s).Must(s =>

                !string.IsNullOrEmpty(s.ParishId) ||
                s.ParishIds?.Length != 0 ||
                !string.IsNullOrEmpty(s.Location)
                )
                .WithMessage(_localizer["Search Validation At Least One Message"]);
        }

    }
}