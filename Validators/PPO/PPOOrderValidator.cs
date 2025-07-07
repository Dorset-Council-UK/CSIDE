using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Validators.PPO
{
    public class PPOOrderValidator : AbstractValidator<PPOOrder>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public PPOOrderValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(o => o.ObjectionsWithdrawn).NotEmpty().When(o => o.ObjectionsReceived == true);


        }
    }
}
