using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Data.Validators.PPO
{
    public class PPOOrderValidator : AbstractValidator<PPOOrder>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public PPOOrderValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(o => o.ObjectionsWithdrawn).NotEmpty().When(o => o.ObjectionsReceived == true);


        }
    }
}
