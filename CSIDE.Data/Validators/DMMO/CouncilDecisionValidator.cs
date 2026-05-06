using FluentValidation;
using CSIDE.Data.Models.DMMO;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.DMMO
{
    public class CouncilDecisionValidator : AbstractValidator<DMMOCouncilDecision>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public CouncilDecisionValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;
            RuleFor(dmmoCouncilDecision => dmmoCouncilDecision.CouncilDecisionType)
                .NotEmpty()
                .WithName(_localizer["Council Decision Type Label"]);

            RuleFor(dmmoCouncilDecision => dmmoCouncilDecision.Date)
                .NotEmpty()
                .WithName(_localizer["Council Decision Date Label"]);

            RuleFor(dmmoCouncilDecision => dmmoCouncilDecision.Notes)
                .MaximumLength(4000)
                .WithName(_localizer["Council Decision Notes Label"]);
        }
    }
}
