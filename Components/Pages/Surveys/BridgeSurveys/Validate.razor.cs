using AutoMapper;
using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Models.Surveys;
using CSIDE.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjNet.CoordinateSystems;

namespace CSIDE.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Validate(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        NavigationManager navigationManager,
        ILogger<Validate> logger,
        IMapper mapper,
        ISettingsService settingsService)
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public int SurveyId { get; init; }

        private BridgeSurvey? Survey { get; set; }
        private bool SurveyHasUpdatedLocation { get; set; }

        private bool UpdateBridgeDetails { get; set; }
        private bool UpdateBridgeLocation { get; set; }
        private bool AddMediaToBridge { get; set; }
        private bool CreateMaintJob { get; set; }
        private bool? ApproveSurvey { get; set; } = true;

        public string? ErrorMessage { get; set; }
        public bool IsBusy { get; set; }

        private List<BreadcrumbItem>? NavItems;


        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
                new BreadcrumbItem{ Text = localizer["Bridge Survey Details Title", SurveyId], IsCurrentPage = true },
            ];
        }

        protected override async Task OnParametersSetAsync()
        {
            //get survey
            using var context = contextFactory.CreateDbContext();
            Survey = await  context.BridgeSurveys
                .FindAsync(SurveyId);
            if (Survey is null)
            {
                navigationManager.NavigateTo("/surveys/bridge/new");
                return;
            }
            if(Survey.Status is SurveyStatus.Incomplete)
            {
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/summary");
                return;
            }
            if (Survey.Status is SurveyStatus.Verified or SurveyStatus.Rejected)
            {
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/details");
            }
            SurveyHasUpdatedLocation = (Survey.UpdatedX is not null && Survey.UpdatedY is not null && Survey.LocationAccuracy is not null);

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Survey is not null)
            {
                //add to recent work store
                await settingsService.AddRecentWork($"{IDPrefixOptions.Value.Infrastructure}/{Survey.InfrastructureItemId}", "Survey", Survey.Status.Humanize(), $"surveys/bridge/{Survey.Id}/details");
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        public async Task SubmitFormAsync()
        {
            if (Survey is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            IsBusy = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                //get the existing job to enable the smarter change tracker.
                //Without this, all properties are identified as tracked, since
                //the DbContext is different from when the entity was queried
                var existingSurvey = await context.BridgeSurveys.IgnoreAutoIncludes().Where(s => s.Id == SurveyId).FirstAsync() ?? throw new Exception($"Survey being edited (ID: {SurveyId}) was not found prior to updating");
                if (ApproveSurvey.HasValue && ApproveSurvey.Value == true)
                {
                    Survey.Status = SurveyStatus.Verified;
                }
                else
                {
                    Survey.Status = SurveyStatus.Rejected;
                }
                
                context.Entry(existingSurvey).CurrentValues.SetValues(Survey);

                var infra = await context.Infrastructure.FindAsync(Survey.InfrastructureItemId);
                EntityEntry<Job>? createdMaintJob = null;
                if (infra is not null)
                {
                    if (UpdateBridgeDetails)
                    {
                        if (infra is null)
                        {
                            throw new Exception($"Infrastructure item (ID: {Survey.InfrastructureItemId}) was not found prior to updating");
                        }
                        if (infra.BridgeDetails is null)
                        {
                            // Create a new InfrastructureBridgeDetails instance
                            var mapped = mapper.Map<InfrastructureBridgeDetails>(Survey);
                            infra.BridgeDetails = mapped;
                            infra.BridgeDetails.InfrastructureId = infra.Id;
                            context.InfrastructureBridgeDetails.Add(infra.BridgeDetails);
                        }
                        else
                        {
                            mapper.Map(Survey, infra.BridgeDetails);
                        }
                        infra.Width = Survey.Width.HasValue ? Convert.ToDouble(Survey.Width.Value) : infra.Width;
                        infra.Height = Survey.Height.HasValue ? Convert.ToDouble(Survey.Height.Value) : infra.Height;
                        infra.Length = Survey.Length.HasValue ? Convert.ToDouble(Survey.Length.Value) : infra.Length;
                    }
                    //Update bridge details and location
                    if (UpdateBridgeLocation && SurveyHasUpdatedLocation)
                    {
                        var sql = @"SELECT ST_Transform(ST_SetSRID(ST_MakePoint({0}, {1}), 4326), 27700) AS ""Geom""";

                        // Execute the query and map the result to the GeometryResult class
                        var transformedPoint = await context.Database
                            .SqlQueryRaw<PointGeometryResult>(sql, Survey.UpdatedX!.Value, Survey.UpdatedY!.Value)
                            .FirstOrDefaultAsync();

                        if (transformedPoint?.Geom != null)
                        {
                            infra.Geom = transformedPoint.Geom;
                        }
                    }
                    //Add media
                    if (AddMediaToBridge)
                    {
                        foreach (var media in Survey.SurveyMedia)
                        {
                            if (media is null)
                            {
                                continue;
                            }
                            var infraMedia = new InfrastructureMedia
                            {
                                InfrastructureItemId = infra.Id,
                                MediaId = media.MediaId
                            };
                            context.InfrastructureMedia.Add(infraMedia);
                        }
                    }
                    //Create maint job
                    if(CreateMaintJob && !string.IsNullOrEmpty(Survey.RepairsRequired))
                    {
                        var DefaultJobStatus = await context.MaintenanceJobStatuses.OrderBy(s => s.SortOrder).FirstAsync();
                        var DefaultJobPriority = await context.MaintenanceJobPriorities.OrderBy(s => s.SortOrder).FirstAsync();
                        var maintJob = new Data.Models.Maintenance.Job
                        {
                            Geom = infra.Geom,
                            ProblemDescription = Survey.RepairsRequired,
                            JobStatusId = DefaultJobStatus.Id,
                            JobPriorityId = DefaultJobPriority.Id,
                        };
                        var jobInfra = new JobInfrastructure
                        {
                            InfrastructureId = infra.Id,
                            Job = maintJob
                        };
                        createdMaintJob = context.MaintenanceJobs.Add(maintJob);
                        context.MaintenanceJobInfrastructure.Add(jobInfra);
                    }
                }
                await context.SaveChangesAsync();
                if(createdMaintJob is not null)
                {
                    navigationManager.NavigateTo($"maintenance/details/{createdMaintJob.Entity.Id}");
                    return;
                }
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/details");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating survey");
                ErrorMessage = localizer["Save Error Message"];
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
