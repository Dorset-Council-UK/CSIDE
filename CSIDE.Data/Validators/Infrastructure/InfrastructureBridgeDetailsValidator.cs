using CSIDE.Data.Models.Infrastructure;
using FluentValidation;

namespace CSIDE.Data.Validators.Infrastructure
{
    public class InfrastructureBridgeDetailsValidator : AbstractValidator<InfrastructureBridgeDetails?>
    {
        public InfrastructureBridgeDetailsValidator()
        {
            RuleFor(b => b!.NumBeamTimbers)
                .LessThanOrEqualTo(100);
            RuleFor(b => b!.NumDeckingBoards)
                .LessThanOrEqualTo(100);
            RuleFor(b => b!.NumHandrailPostsTimbers)
                .LessThanOrEqualTo(100);
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
                .When(b => b!.DeckingBoardsLength.HasValue);
        }
    }
}
