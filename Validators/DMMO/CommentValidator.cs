using FluentValidation;
using CSIDE.Data.Models.DMMO;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.DMMO
{
    public class CommentValidator : AbstractValidator<DMMOComment>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public CommentValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;
            RuleFor(comment => comment.CommentText)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Comment Label"]);

            RuleFor(comment => comment.CommentDate)
                .NotEmpty()
                .WithName(_localizer["Comment Date Label"]);
        }
    }
}
