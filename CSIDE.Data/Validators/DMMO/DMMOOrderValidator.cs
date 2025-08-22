using FluentValidation;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.DMMO;

namespace CSIDE.Data.Validators.DMMO
{
    public class DMMOOrderValidator : AbstractValidator<DMMOOrder>
    {
        readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        public DMMOOrderValidator(IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(o => o.ObjectionsWithdrawn).NotEmpty().When(o => o.ObjectionsReceived == true);


        }
    }
}
