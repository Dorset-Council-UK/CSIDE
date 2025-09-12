using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.Infrastructure
{
    public class InfrastructureItemValidator : AbstractValidator<InfrastructureItem>
    {
        private readonly IRightsOfWayService _rightsOfWayService;
        private readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;

        public InfrastructureItemValidator(IRightsOfWayService rightsOfWayService, IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer)
        {
            _rightsOfWayService = rightsOfWayService;
            _localizer = localizer;

            RuleFor(item => item.Description)
                .MaximumLength(4000)
                .WithName(_localizer["Infrastructure Description Label"]);
            RuleFor(item => item.InfrastructureTypeId)
                .NotEmpty()
                .WithName(_localizer["Infrastructure Type Label"]);
            RuleFor(item => item.Height)
                .LessThanOrEqualTo(100)
                .GreaterThanOrEqualTo(0)
                .WithName(_localizer["Infrastructure Height Label"]);
            RuleFor(item => item.Length)
                .LessThanOrEqualTo(500)
                .GreaterThanOrEqualTo(0)
                .WithName(_localizer["Infrastructure Length Label"]);
            RuleFor(item => item.Width)
                .LessThanOrEqualTo(100)
                .GreaterThanOrEqualTo(0)
                .WithName(_localizer["Infrastructure Width Label"]);
            RuleFor(item => item.RouteId)
                .NotEmpty()
                .WithName(_localizer["Route ID Label"])
                .MustAsync(RouteExists)
                .WithMessage(r => _localizer["Route Does Not Exist Validation Message",r.RouteId!]);

            RuleFor(item => item.BridgeDetails)
                .SetValidator(new InfrastructureBridgeDetailsValidator())
                .When(item => item.BridgeDetails != null);
        }

        private async Task<bool> RouteExists(string? routeCode, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
            {
                return false;
            }
            return await _rightsOfWayService.RouteExists(routeCode, ct);
        }
    }
}
