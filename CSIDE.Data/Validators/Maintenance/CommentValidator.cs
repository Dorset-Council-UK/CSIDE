using FluentValidation;
using CSIDE.Data.Models.Maintenance;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.Maintenance
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public CommentValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;
            RuleFor(comment => comment.CommentText)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Comment Label"]);
        }
    }
}
