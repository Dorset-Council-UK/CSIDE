using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.RightsOfWay
{
    public class RouteValidator : AbstractValidator<Models.RightsOfWay.Route>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        private readonly IRightsOfWayService _rightsOfWayService;

        public RouteValidator(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer,
            IRightsOfWayService rightsOfWayService
        ) {
            _contextFactory = contextFactory;
            _localizer = localizer;
            _rightsOfWayService = rightsOfWayService;

            RuleSet("New Route", () =>
            {
                RuleFor(route => route.RouteCode)
                .NotEmpty()
                .MustAsync(HasUniqueRouteCode)
                .WithMessage(route => _localizer["Route Code Not Unique Validation Message", route.RouteCode]);
            });

            // This is validated within the GeometryValidator rulesets, but the NotEmpty check helps catch when no geometry is provided at all.
            RuleFor(route => route.Geom)
                .NotEmpty()
                .WithMessage(localizer["Invalid Geometry Validation Message"]);
            
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
                .LessThan(route => route.ClosureEndDate)
                .WithMessage(localizer["Invalid Closure Dates Validation Message"])
                .Unless(route => route.ClosureIsIndefinite)
                .NotEmpty()
                .WhenAsync(RouteStatusIsClosed)
                .WithName(_localizer["Closure Start Date Label"]);
            RuleFor(route => route.ClosureEndDate)
                .GreaterThan(route => route.ClosureStartDate)
                .WithMessage(localizer["Invalid Closure Dates Validation Message"])
                .Unless(route => route.ClosureIsIndefinite)
                .NotEmpty()
                .WhenAsync(RouteStatusIsClosed)
                .Unless(route => route.ClosureIsIndefinite)
                .WithName(_localizer["Closure End Date Label"]);
        }

        private async Task<bool> RouteStatusIsClosed(Models.RightsOfWay.Route route, CancellationToken ct)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            var operationalStatus = await context.RouteOperationalStatuses
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Where(os => os.Id == route.OperationalStatusId)
                .Select(os => new { os.Id, os.IsClosed })
                .FirstOrDefaultAsync(ct);

            return operationalStatus?.IsClosed ?? false;
        }

        private async Task<bool> HasUniqueRouteCode(string routeCode, CancellationToken ct)
        {
            var exists = await _rightsOfWayService.RouteExists(routeCode, ct);
            return !exists;
        }
    }
}
