using CSIDE.Data.Models.Surveys;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.BridgeSurveys
{
    public class BridgeSurveyValidator : AbstractValidator<BridgeSurvey>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public BridgeSurveyValidator(IStringLocalizer<Properties.Resources> localizer) {
            _localizer = localizer;
            RuleFor(b => b.NumBeamTimbers)
                .LessThanOrEqualTo(100);
            RuleFor(b => b.NumDeckingBoards)
                .LessThanOrEqualTo(100);
            RuleFor(b => b.NumHandrailPostsTimbers)
                .LessThanOrEqualTo(100);
            RuleFor(b => b.BeamTimbersSize)
                    .MaximumLength(20);
            RuleFor(b => b.DeckingBoardsSize)
                    .MaximumLength(20);
            RuleFor(b => b.HandrailTimbersSize)
                    .MaximumLength(20);
            RuleFor(b => b.HandrailPostsTimbersSize)
                    .MaximumLength(20);
            RuleFor(b => b.DeckingBoardsLength)
                .LessThan(100)
                .When(b => b.DeckingBoardsLength.HasValue);
            RuleFor(b => b.Height)
                .LessThanOrEqualTo(100)
                .GreaterThanOrEqualTo(0)
                .WithName(_localizer["Infrastructure Height Label"]);
            RuleFor(b => b.Length)
                .LessThanOrEqualTo(500)
                .GreaterThanOrEqualTo(0)
                .WithName(_localizer["Infrastructure Length Label"]);
            RuleFor(b => b.Width)
                .LessThanOrEqualTo(100)
                .GreaterThanOrEqualTo(0)
                .WithName(_localizer["Infrastructure Width Label"]);
            RuleFor(x => x.RepairsRequired)
                .MaximumLength(4000)
                .WithName(_localizer["Bridge Survey Repairs Required Label"]);


        }

    }
}
