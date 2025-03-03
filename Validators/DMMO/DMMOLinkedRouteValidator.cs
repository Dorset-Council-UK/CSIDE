using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.DMMO
{
    public class DMMOLinkedRouteValidator : AbstractValidator<DMMOLinkedRoute>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        readonly IRightsOfWayHelperService _rightsOfWayHelper;

        public DMMOLinkedRouteValidator(IDbContextFactory<ApplicationDbContext> contextFactory,
            IStringLocalizer<Properties.Resources> localizer,
            IRightsOfWayHelperService rightsOfWayHelperService)
        {
            _localizer = localizer;
            _contextFactory = contextFactory;
            _rightsOfWayHelper = rightsOfWayHelperService;
            RuleFor(d => d.RouteId)
                .NotEmpty().WithName(_localizer["Route ID Label"])
                .MustAsync((dmmoLinkedRoute, RouteId, ct) => RouteIDNotAlreadyLinked(RouteId, dmmoLinkedRoute.ApplicationId, ct))
                .WithMessage(_localizer["Linked Route Already Exists Validation Message"])
                .MustAsync(RouteIDExists)
                .WithMessage(r => _localizer["Route Does Not Exist Validation Message", r.RouteId]);
                
            RuleFor(d => d.ApplicationId)
                .NotEmpty().WithName(_localizer["Application ID Label"])
                .MustAsync(DMMOApplicationExists).WithMessage(_localizer["DMMO Not Found Error Message"]);
        }

        private async Task<bool> RouteIDNotAlreadyLinked(string RouteId, int ApplicationId, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var DMMOLinkedRoute = await context.DMMOLinkedRoutes.FindAsync([ApplicationId, RouteId], cancellationToken: ct);
            return (DMMOLinkedRoute is null); 
        }

        private async Task<bool> RouteIDExists(string RouteId, CancellationToken ct)
        {
            return await _rightsOfWayHelper.RouteExistsAsync(RouteId);
        }

        private async Task<bool> DMMOApplicationExists(int ApplicationId, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var DMMOApplication = await context.DMMOApplication.FindAsync([ApplicationId], cancellationToken: ct);
            return (DMMOApplication is not null);
        }

    }
}
