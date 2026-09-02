using CSIDE.Data.Models.Infrastructure;
using FluentValidation;

namespace CSIDE.Data.Validators.Infrastructure
{
    public class InfrastructureBridgeDetailsValidator : AbstractValidator<InfrastructureBridgeDetails?>
    {
        public InfrastructureBridgeDetailsValidator()
        {
            RuleFor(b => b!.NumBeamTimbers)
                .LessThanOrEqualTo(1000)
                .GreaterThanOrEqualTo(0);
            RuleFor(b => b!.NumDeckingBoards)
                .LessThanOrEqualTo(1000)
                .GreaterThanOrEqualTo(0);
            RuleFor(b => b!.NumHandrailPostsTimbers)
                .LessThanOrEqualTo(1000)
                .GreaterThanOrEqualTo(0);
            RuleFor(b => b!.BeamTimbersSize)
                    .MaximumLength(20);
            RuleFor(b => b!.DeckingBoardsSize)
                    .MaximumLength(20);
            RuleFor(b => b!.HandrailTimbersSize)
                    .MaximumLength(20);
            RuleFor(b => b!.HandrailPostsTimbersSize)
                    .MaximumLength(20);
            RuleFor(b => b!.DeckingBoardsLength)
                .LessThan(100)
                .GreaterThanOrEqualTo(0)
                .When(b => b!.DeckingBoardsLength.HasValue);
        }
    }
}
