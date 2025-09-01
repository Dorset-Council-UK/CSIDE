using AutoMapper;
using BlazorBootstrap;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using CSIDE.Web.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ProjNet.CoordinateSystems;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Validate(
        IInfrastructureService infrastructureService,
        NavigationManager navigationManager,
        ILogger<Validate> logger,
        IMapper mapper,
        ISettingsService settingsService,
        IMaintenanceJobsService maintenanceJobsService,
        ISharedDataService sharedDataService
    ) {
        [Parameter]
        public int SurveyId { get; init; }

        [CascadingParameter]
        public Task<AuthenticationState>? AuthenticationState { get; set; }

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
            Survey = await infrastructureService.GetBridgeSurveyById(SurveyId);
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
                await settingsService.AddRecentWork($"{IDPrefixOptions.Value.Infrastructure}{Survey.InfrastructureItemId}/{Survey.Id}", "Survey", Survey.Status.Humanize(), $"surveys/bridge/{Survey.Id}/details");
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
                if (ApproveSurvey.HasValue && ApproveSurvey.Value)
                {
                    Survey.Status = SurveyStatus.Verified;
                }
                else
                {
                    Survey.Status = SurveyStatus.Rejected;
                }
                
                await infrastructureService.UpdateBridgeSurvey(SurveyId, Survey);

                var infra = await infrastructureService.GetInfrastructureItemById(Survey.InfrastructureItemId);
                Job? createdMaintJob = null;
                if (infra is not null)
                {
                    if (UpdateBridgeDetails)
                    {
                        if (infra.BridgeDetails is null)
                        {
                            // Create a new InfrastructureBridgeDetails instance
                            var mapped = mapper.Map<InfrastructureBridgeDetails>(Survey);
                            infra.BridgeDetails = mapped;
                            infra.BridgeDetails.InfrastructureId = infra.Id;
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

                        // Execute the query and map the result to the GeometryResult class
                        var transformedPoint = await sharedDataService.TransformCoordinates(Survey.UpdatedX!.Value, Survey.UpdatedY!.Value, 4326,27700);

                        if (transformedPoint?.Geom != null)
                        {
                            infra.Geom = transformedPoint.Geom;
                        }
                    }

                    await infrastructureService.UpdateInfrastructureItem(infra);

                    //Add media
                    if (AddMediaToBridge)
                    {
                        await infrastructureService.AddMediaToInfrastructureItem(infra, [.. Survey.SurveyMedia.Select(m => m.Media)]);
                    }

                    //Create maint job
                    if (CreateMaintJob && !string.IsNullOrEmpty(Survey.RepairsRequired))
                    {
                        var DefaultJobStatus = (await maintenanceJobsService.GetMaintenanceJobStatuses()).OrderBy(s => s.SortOrder).First();
                        var DefaultJobPriority = (await maintenanceJobsService.GetMaintenanceJobPriorities()).OrderBy(s => s.SortOrder).First();
                        var maintJob = new Job
                        {
                            Geom = infra.Geom,
                            ProblemDescription = Survey.RepairsRequired,
                            JobStatusId = DefaultJobStatus.Id,
                            JobPriorityId = DefaultJobPriority.Id,
                        };
                        if (AuthenticationState != null)
                        {
                            var authState = await AuthenticationState;
                            maintJob.LoggedById = authState.GetUserId();
                            maintJob.LoggedByName = authState.GetUserName();
                        }
                        
                        createdMaintJob = await maintenanceJobsService.CreateMaintenanceJob(maintJob, []);
                        await maintenanceJobsService.AddInfrastructureToJob(createdMaintJob, infra);
                    }
                }
                if(createdMaintJob is not null)
                {
                    navigationManager.NavigateTo($"maintenance/details/{createdMaintJob.Id}");
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
