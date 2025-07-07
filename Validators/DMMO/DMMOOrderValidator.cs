using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Validators.DMMO
{
    public class DMMOOrderValidator : AbstractValidator<DMMOOrder>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public DMMOOrderValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(o => o.ObjectionsWithdrawn).NotEmpty().When(o => o.ObjectionsReceived == true);


        }
    }
}
