using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Services;
using CSIDE.Shared.Properties;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NodaTime;

namespace CSIDE.Data.Validators.Maintenance
{
    public class JobValidator : AbstractValidator<Job>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IMaintenanceJobsService _maintenanceJobsService;
        private readonly IRightsOfWayService _rightsOfWayService;

        public JobValidator(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IStringLocalizer<Resources> localizer,
            IMaintenanceJobsService maintenanceJobsService,
            IRightsOfWayService rightsOfWayService
        ) {
            _contextFactory = contextFactory;
            _localizer = localizer;
            _maintenanceJobsService = maintenanceJobsService;
            _rightsOfWayService = rightsOfWayService;

            // This is validated within the GeometryValidator rulesets, but the NotEmpty check helps catch when no geometry is provided at all.
            RuleFor(job => job.Geom)
                .NotEmpty()
                .WithMessage(localizer["Invalid Geometry Validation Message"])
                .WithErrorCode("INVALID_GEOM");

            RuleFor(job => job.ProblemDescription)
                .NotEmpty()
                .MaximumLength(4000)
                .WithName(_localizer["Problem Description Label"]);
            RuleFor(job => job.WorkDone)
                .MaximumLength(4000)
                .WithName(_localizer["Work Done Label"]);
            RuleFor(job => job.JobStatusId)
                .NotEmpty()
                .WithName(_localizer["Job Status Label"]);
            RuleFor(job => job.JobPriorityId)
                .NotEmpty()
                .WithName(_localizer["Job Priority Label"]);
            RuleFor(job => job.RouteId)
                .NotEmpty()
                .WithName(_localizer["Route ID Label"])
                .MustAsync(RouteExists)
                .WithMessage(r => _localizer["Route Does Not Exist Validation Message",r.RouteId!]);
                
            RuleFor(job => job.CompletionDate)
                .NotEmpty()
                .LessThanOrEqualTo(LocalDate.FromDateTime(DateTime.Now))
                .WhenAsync(JobStatusIsComplete)
                .WithName(_localizer["Completion Date Label"]);

            RuleFor(job => job.WorkDone)
                .NotEmpty()
                .WhenAsync(JobStatusIsComplete)
                .UnlessAsync(JobStatusIsDuplicate)
                .WithName(_localizer["Work Done Label"]);

            RuleFor(job => job.DuplicateJobId)
                .NotEmpty()
                .WithName(_localizer["Duplicate Job ID Label"])
                .MustAsync(JobExists)
                .WithMessage(r => _localizer["Maintenance Job Not Found Error Message", r.DuplicateJobId!])
                .WhenAsync(JobStatusIsDuplicate);
        }

        private async Task<bool> JobStatusIsComplete(Job job, CancellationToken ct)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            return await context.MaintenanceJobStatuses
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Where(o => o.Id == job.JobStatusId)
                .Select(o => o.IsComplete)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        private async Task<bool> JobStatusIsDuplicate(Job job, CancellationToken ct)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(ct);
            return await context.MaintenanceJobStatuses
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Where(o => o.Id == job.JobStatusId)
                .Select(o => o.IsDuplicate)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        private async Task<bool> RouteExists(string? routeCode, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
            {
                return false;
            }
            return await _rightsOfWayService.RouteExists(routeCode, ct);
        }

        private async Task<bool> JobExists(int? jobId, CancellationToken ct)
        {
            if (jobId == null)
            {
                return false;
            }
            return await _maintenanceJobsService.MaintenanceJobExists(jobId.Value, ct);
        }
    }
}
