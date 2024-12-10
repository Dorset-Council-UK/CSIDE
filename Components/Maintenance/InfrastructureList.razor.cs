using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Maintenance;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Components.Maintenance
{
    public partial class InfrastructureList(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS)
    {
        [Parameter]
        public ICollection<JobInfrastructure>? JobInfrastructure { get; set; }
        [Parameter]
        public required int JobId { get; init; }
        [Parameter]
        public bool IsEditable { get; set; }

        public bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }

        private List<InfrastructureWithDistance> NearbyInfra { get; set; } = [];
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
            using var context = contextFactory.CreateDbContext();
            var job = await context.MaintenanceJobs.IgnoreAutoIncludes().Where(j => j.Id == JobId).FirstOrDefaultAsync();
            if (job is not null)
            {
                //get nearest 10 infra items within 50m of job
                var existingIds = JobInfrastructure?.Select(j => j.InfrastructureId);
                NearbyInfra = await context.Infrastructure
                    .Where(i => i.Geom.IsWithinDistance(job.Geom, 50))
                    .OrderBy(i => i.Geom.Distance(job.Geom))
                    .Select(i => new InfrastructureWithDistance() { Infrastructure = i, Distance = i.Geom.Distance(job.Geom), AlreadyLinked = existingIds == null || (existingIds.Contains(i.Id)) })
                    .ToListAsync();
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
                    using var context = contextFactory.CreateDbContext();
                    context.Add(NewJobInfrastructure);
                    await context.SaveChangesAsync();
                    await AddInfraLinkModal.HideAsync();
                    await RefreshComponent();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = localizer["Save Error Message"];
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
                using var context = contextFactory.CreateDbContext();
                context.Add(InfraJobToAdd);
                await context.SaveChangesAsync();
                infra.AlreadyLinked = true;
                await RefreshComponent();

            }
            catch (Exception ex)
            {
                ErrorMessage = localizer["Save Error Message"];
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteInfraLink(int InfrastructureId, int JobId)
        {
            IsBusy = true;
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Infra Link Confirmation"].Value);
            if (ConfirmDelete)
            {
                using var context = contextFactory.CreateDbContext();
                var infraToDelete = await context.MaintenanceJobInfrastructure.FindAsync([JobId, InfrastructureId]);
                if (infraToDelete is not null)
                {
                    context.Remove(infraToDelete);
                    await context.SaveChangesAsync();
                    await RefreshComponent();
                }
            }
            IsBusy = false;
        }

        private async Task RefreshComponent()
        {
            using var context = contextFactory.CreateDbContext();
            JobInfrastructure = await context.MaintenanceJobInfrastructure.Where(j => j.JobId == JobId).ToListAsync();
        }
    }
}
