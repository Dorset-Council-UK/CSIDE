using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CSIDE.Data.Validators.Maintenance
{
    public class JobInfrastructureValidator : AbstractValidator<JobInfrastructure>
    {
        private readonly IMaintenanceJobsService _maintenanceJobsService;
        private readonly IInfrastructureService _infrastructureService;

        public JobInfrastructureValidator(
            IStringLocalizer<CSIDE.Shared.Properties.Resources> localizer,
            IMaintenanceJobsService maintenanceJobsService,
            IInfrastructureService infrastructureService
        ) {
            _maintenanceJobsService = maintenanceJobsService;
            _infrastructureService = infrastructureService;

            RuleFor(ji => ji.InfrastructureId)
                .NotEmpty()
                .WithName(localizer["Infrastructure ID Label"])
                .MustAsync(InfrastructureExists)
                .WithMessage(localizer["Infrastructure Does Not Exist Validation Message"]);
            RuleFor(ji => ji.JobId)
                .NotEmpty()
                .WithName(localizer["Job ID Label"])
                .MustAsync(JobIDExists)
                .WithMessage(localizer["Maintenance Job Not Found Error Message"]);
        }

        private async Task<bool> InfrastructureExists(int infrastructureId, CancellationToken ct)
        {
            return await _infrastructureService.InfrastructureItemExists(infrastructureId, ct);
        }

        private async Task<bool> JobIDExists(int jobId, CancellationToken ct)
        {
            return await _maintenanceJobsService.MaintenanceJobExists(jobId, ct);
        }
    }
}
