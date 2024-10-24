using FluentValidation;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CSIDE.Validators.Maintenance
{
    public class JobValidator : AbstractValidator<Job>
    {
        readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        readonly IStringLocalizer<Properties.Resources> _localizer;
        public JobValidator(IDbContextFactory<ApplicationDbContext> contextFactory, IStringLocalizer<Properties.Resources> localizer)
        {
            _contextFactory = contextFactory;
            _localizer = localizer;
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
                .WithName(_localizer["Route ID Label"]);
            RuleFor(job => job.CompletionDate)
                .NotEmpty()
                .WhenAsync(JobStatusIsComplete)
                .WithName(_localizer["Completion Date Label"]);
        }

        private async Task<bool> JobStatusIsComplete(Job job, CancellationToken ct)
        {
            using var context = _contextFactory.CreateDbContext();
            var JobStatus = await context.JobStatuses.FindAsync([job.JobStatusId], cancellationToken: ct);
            if(JobStatus is not null)
            {
                return JobStatus.IsComplete;
            }
            return false;
        }
    }
}
