using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.RightsOfWay;

namespace CSIDE.Validators.RightsOfWay
{
    public class StatementValidator : AbstractValidator<Statement>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public StatementValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;
  
            RuleFor(statement => statement.StatementText)
                .NotEmpty()
                .WithName(_localizer["Statement Text Label"]);

            RuleFor(statement => statement.StartGridRef)
                .MaximumLength(20);

            RuleFor(statement => statement.EndGridRef)
                .MaximumLength(20);
        }
    }
}
