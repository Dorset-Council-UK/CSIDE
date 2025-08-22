using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NodaTime;

namespace CSIDE.Data.Validators.Maintenance
{
    public class JobValidator : AbstractValidator<Job>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IStringLocalizer<CSIDE.Shared.Properties.Resources> _localizer;
        private readonly IMaintenanceJobsService _maintenanceJobsService;

        public JobValidator(IDbContextFactory<ApplicationDbContext> contextFactory, IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer, IMaintenanceJobsService maintenanceJobsService)
        {
            _contextFactory = contextFactory;
            _localizer = localizer;
            _maintenanceJobsService = maintenanceJobsService;

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
                .NotEmpty().WithName(_localizer["Route ID Label"])
                .MustAsync(RouteIDExists).WithMessage(r => _localizer["Route Does Not Exist Validation Message",r.RouteId!]);
                
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
                .NotEmpty().WithName(_localizer["Duplicate Job ID Label"])
                .MustAsync(JobIDExists).WithMessage(r => _localizer["Maintenance Job Not Found Error Message", r.DuplicateJobId!])
                .WhenAsync(JobStatusIsDuplicate);
        }

        private async Task<bool> JobStatusIsComplete(Job job, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var JobStatus = await context.MaintenanceJobStatuses.FindAsync([job.JobStatusId], cancellationToken: ct);
            if(JobStatus is not null)
            {
                return JobStatus.IsComplete;
            }
            return false;
        }

        private async Task<bool> JobStatusIsDuplicate(Job job, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var JobStatus = await context.MaintenanceJobStatuses.FindAsync([job.JobStatusId], cancellationToken: ct);
            if (JobStatus is not null)
            {
                return JobStatus.IsDuplicate;
            }
            return false;
        }

        private async Task<bool> RouteIDExists(string? RouteId, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var Route = await context.Routes.FindAsync([RouteId], cancellationToken: ct);
            return (Route is not null);
        }

        private async Task<bool> JobIDExists(int? JobId, CancellationToken ct)
        {
            if (JobId == null)
            {
                return false;
            }

            var job = await _maintenanceJobsService.GetMaintenanceJobById(JobId.Value, ct);
            return (job != null);
        }
    }
}
