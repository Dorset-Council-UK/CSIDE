using FluentValidation;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.Maintenance
{
    public class CommentValidator : AbstractValidator<Comment>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public CommentValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;
            RuleFor(comment => comment.CommentText)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Comment Label"]);
        }
    }
}
