using FluentValidation;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CSIDE.Data.Models.RightsOfWay;

namespace CSIDE.Validators.RightsOfWay
{
    public class RouteValidator : AbstractValidator<Data.Models.RightsOfWay.Route>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public RouteValidator(IDbContextFactory<ApplicationDbContext> contextFactory, IStringLocalizer<Properties.Resources> localizer)
        {
            _contextFactory = contextFactory;
            _localizer = localizer;
            RuleSet("New Route", () =>
            {
                RuleFor(route => route.RouteCode)
                .NotEmpty()
                .MustAsync(HasUniqueRouteCode)
                .WithMessage(route => _localizer["Route Code Not Unique Validation Message", route.RouteCode]);
            });
                
            RuleFor(route => route.Name)
                .MaximumLength(200)
                .WithName(_localizer["Name Label"]);

            RuleFor(route => route.RouteTypeId)
                .NotEmpty()
                .WithName(_localizer["Route Type Label"]);
            RuleFor(route => route.OperationalStatusId)
                .NotEmpty()
                .WithName(_localizer["Operational Status Label"]);
            RuleFor(route => route.LegalStatusId)
                .NotEmpty()
                .WithName(_localizer["Legal Status Label"]);
            RuleFor(route => route.ClosureStartDate)
                .LessThan(route => route.ClosureEndDate).WithMessage(localizer["Invalid Closure Dates Validation Message"])
                .Unless(route => route.ClosureIsIndefinite)
                .NotEmpty()
                .WhenAsync(RouteStatusIsClosed)
                .WithName(_localizer["Closure Start Date Label"]);
            RuleFor(route => route.ClosureEndDate)
                .GreaterThan(route => route.ClosureStartDate).WithMessage(localizer["Invalid Closure Dates Validation Message"])
                .Unless(route => route.ClosureIsIndefinite)
                .NotEmpty()
                .WhenAsync(RouteStatusIsClosed)
                .Unless(route => route.ClosureIsIndefinite)
                .WithName(_localizer["Closure End Date Label"]);


        }

        private async Task<bool> RouteStatusIsClosed(Data.Models.RightsOfWay.Route route, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var OperationalStatus = await context.RouteOperationalStatuses.FindAsync([route.OperationalStatusId], cancellationToken: ct);
            if (OperationalStatus is not null)
            {
                return OperationalStatus.IsClosed;
            }
            return false;
        }

        private async Task<bool> HasUniqueRouteCode(string RouteCode, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var Route = await context.Routes.FindAsync([RouteCode], cancellationToken: ct);
            return Route is null;
        }

    }
}
