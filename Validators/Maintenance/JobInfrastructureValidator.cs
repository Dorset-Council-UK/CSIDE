using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.Maintenance
{
    public class JobInfrastructureValidator : AbstractValidator<JobInfrastructure>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        
        public JobInfrastructureValidator(IDbContextFactory<ApplicationDbContext> contextFactory,IStringLocalizer<Properties.Resources> localizer)
        {
            _localizer = localizer;
            _contextFactory = contextFactory;
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
            using var context = _contextFactory.CreateDbContext();
            var Job = await context.MaintenanceJobs.FindAsync([JobId], cancellationToken: ct);
            return (Job is not null);
        }

    }
}
