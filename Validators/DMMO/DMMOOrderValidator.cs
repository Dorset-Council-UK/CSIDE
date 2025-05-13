using FluentValidation;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.DMMO;
using NodaTime;

namespace CSIDE.Validators.DMMO
{
    public class DMMOOrderValidator : AbstractValidator<Order>
    {
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public DMMOOrderValidator(IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;

            RuleFor(o => o.ObjectionsWithdrawn).NotEmpty().When(o => o.ObjectionsReceived == true);


        }
    }
}
