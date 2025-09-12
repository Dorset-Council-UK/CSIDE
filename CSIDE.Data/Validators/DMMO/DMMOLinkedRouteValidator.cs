using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.DMMO
{
    public class DMMOLinkedRouteValidator : AbstractValidator<DMMOLinkedRoute>
    {
        private readonly IDMMOService _dmmoService;
        private readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        private readonly IRightsOfWayService _rightsOfWayHelper;

        public DMMOLinkedRouteValidator(
            IDMMOService dmmoService, 
            IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer,
            IRightsOfWayService rightsOfWayHelperService)
        {
            _dmmoService = dmmoService;
            _localizer = localizer;
            _rightsOfWayHelper = rightsOfWayHelperService;

            RuleFor(d => d.RouteId)
                .NotEmpty()
                .WithName(_localizer["Route ID Label"])
                .MustAsync((dmmoLinkedRoute, RouteId, ct) => RouteIDNotAlreadyLinked(RouteId, dmmoLinkedRoute.ApplicationId, ct))
                .WithMessage(_localizer["Linked Route Already Exists Validation Message"])
                .MustAsync(RouteIDExists)
                .WithMessage(r => _localizer["Route Does Not Exist Validation Message", r.RouteId]);
                
            RuleFor(d => d.ApplicationId)
                .NotEmpty()
                .WithName(_localizer["Application ID Label"])
                .MustAsync(DMMOApplicationExists)
                .WithMessage(_localizer["DMMO Not Found Error Message"]);
        }

        private async Task<bool> RouteIDNotAlreadyLinked(string RouteId, int ApplicationId, CancellationToken ct)
        {
            var routes = await _dmmoService.GetDMMOLinkedRoutesByApplicationId(ApplicationId, ct);
            return routes.Any(r => r.RouteId == RouteId) is false;
        }

        private async Task<bool> RouteIDExists(string RouteId, CancellationToken ct)
        {
            return await _rightsOfWayHelper.RouteExists(RouteId, ct);
        }

        private async Task<bool> DMMOApplicationExists(int applicationId, CancellationToken ct)
        {
            return await _dmmoService.ApplicationExists(applicationId, ct);
        }

    }
}
