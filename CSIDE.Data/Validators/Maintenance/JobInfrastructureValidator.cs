using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.Maintenance
{
    public class JobInfrastructureValidator : AbstractValidator<JobInfrastructure>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IMaintenanceJobsService _maintenanceJobsService;

        public JobInfrastructureValidator(IDbContextFactory<ApplicationDbContext> contextFactory, IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer, IMaintenanceJobsService maintenanceJobsService)
        {
            _contextFactory = contextFactory;
            _maintenanceJobsService = maintenanceJobsService;

            RuleFor(ji => ji.InfrastructureId)
                .NotEmpty().WithName(localizer["Infrastructure ID Label"])
                .MustAsync(InfrastructureExists).WithMessage(localizer["Infrastructure Does Not Exist Validation Message"]);
            RuleFor(ji => ji.JobId)
                .NotEmpty().WithName(localizer["Job ID Label"])
                .MustAsync(JobIDExists).WithMessage(localizer["Maintenance Job Not Found Error Message"]);
        }

        private async Task<bool> InfrastructureExists(int InfrastructureId, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var InfrastructureItem = await context.Infrastructure.FindAsync([InfrastructureId], cancellationToken: ct);
            return (InfrastructureItem is not null);
        }

        private async Task<bool> JobIDExists(int JobId, CancellationToken ct)
        {
            var job = await _maintenanceJobsService.GetMaintenanceJobById(JobId, ct);
            return (job != null);
        }

    }
}
