using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.Maintenance
{
    public partial class InfrastructureList(
        IInfrastructureService infrastructureService,
        IMaintenanceJobsService maintenanceJobsService,
        IJSRuntime JS,
        ILogger<InfrastructureList> logger)
    {
        [Parameter]
        public ICollection<JobInfrastructure>? JobInfrastructure { get; set; }
        [Parameter]
        public required int JobId { get; init; }
        [Parameter]
        public bool IsEditable { get; set; }

        public bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }

        private ICollection<InfrastructureWithDistance> NearbyInfra { get; set; } = [];
        private JobInfrastructure NewJobInfrastructure { get; set; } = default!;
        private Modal AddInfraLinkModal = default!;
        private FluentValidationValidator? JobInfrastructureValidator;

        private async Task OpenAddInfraLinkModal()
        {
            NewJobInfrastructure = new() { JobId = JobId };
            await GetNearbyInfra();
            await AddInfraLinkModal.ShowAsync();
        }

        private async Task GetNearbyInfra()
        {
            //get job location
            var job = await maintenanceJobsService.GetMaintenanceJobById(JobId);
            if (job is not null)
            {
                //get nearest 10 infra items within 50m of job
                var existingIds = JobInfrastructure?.Select(j => j.InfrastructureId);
                var nearestInfra = await infrastructureService.GetNearbyInfrastructure(job.Geom!, 50);
                
                NearbyInfra = [.. nearestInfra.Select(i => new InfrastructureWithDistance() { Infrastructure = i, Distance = i.Geom!.Distance(job.Geom), AlreadyLinked = existingIds == null || (existingIds.Contains(i.Id)) })];
            }
        }

        private async Task SubmitFormAsync()
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                if (await JobInfrastructureValidator!.ValidateAsync())
                {
                    //submit
                    await maintenanceJobsService.AddInfrastructureToJob(NewJobInfrastructure);
                    await AddInfraLinkModal.HideAsync();
                    await RefreshComponent();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = localizer["Save Error Message"];
                logger.LogError(ex, "An error occurred linking a job to an infrastructure item");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddSingleInfraLink(InfrastructureWithDistance infra)
        {
            IsBusy = true;
            ErrorMessage = null;
            try
            {
                //submit
                var InfraJobToAdd = new JobInfrastructure() { InfrastructureId = infra.Infrastructure.Id, JobId = JobId };
                await maintenanceJobsService.AddInfrastructureToJob(InfraJobToAdd);
                infra.AlreadyLinked = true;
                await RefreshComponent();

            }
            catch (Exception ex)
            {
                ErrorMessage = localizer["Save Error Message"];
                logger.LogError(ex, "An error occurred linking a job to an infrastructure item");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteInfraLink(int InfrastructureId, int JobId)
        {
            IsBusy = true;
            try
            {
                bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Infra Link Confirmation"].Value);
                if (ConfirmDelete)
                {
                    await maintenanceJobsService.RemoveInfrastructureFromJob(JobId, InfrastructureId);
                    await RefreshComponent();

                }
            }
            catch (Exception ex)
            {
                ErrorMessage = localizer["Delete Error Message"];
                logger.LogError(ex, "An error occurred deleting a job-infrastructure link");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshComponent()
        {
            JobInfrastructure = await maintenanceJobsService.GetLinkedInfrastructureForJob(JobId);
        }
    }
}
