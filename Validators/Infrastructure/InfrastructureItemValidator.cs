using FluentValidation;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.Infrastructure
{
    public class InfrastructureItemValidator : AbstractValidator<InfrastructureItem>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public InfrastructureItemValidator(IDbContextFactory<ApplicationDbContext> contextFactory, IStringLocalizer<Properties.Resources> localizer)
        {
            _contextFactory = contextFactory;
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
            RuleFor(job => job.RouteId)
                .NotEmpty().WithName(_localizer["Route ID Label"])
                .MustAsync(RouteIDExists).WithMessage(r => _localizer["Route Does Not Exist Validation Message",r.RouteId!]);
        }

        private async Task<bool> RouteIDExists(string? RouteId, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var Route = await context.Routes.FindAsync([RouteId], cancellationToken: ct);
            return (Route is not null);
        }
    }
}
