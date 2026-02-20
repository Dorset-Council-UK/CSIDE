using BlazorBootstrap;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CSIDE.Web.Components.Pages.Surveys
{
    public partial class List(IInfrastructureService infrastructureService,
                              IMaintenanceJobsService maintenanceJobsService,
                              ILogger<List> logger,
                              NavigationManager navigationManager)
    {
        private List<BreadcrumbItem>? NavItems;
        public string? ErrorMessage { get; set; }
        public bool IsBusy { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }
        private ICollection<BridgeSurvey> Surveys { get; set; } = [];
        private HashSet<int> UserMaintenanceTeamIds { get; set; } = [];
        private bool FilterByMyArea { get; set; } = true;
        private Grid<BridgeSurvey>? SurveysAwaitingValidationGrid { get; set; }
        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
                new BreadcrumbItem{ Text = localizer["Existing Bridge Surveys Title"], IsCurrentPage = true },
            ];
            try
            {
                if (AuthenticationState is not null)
                {
                    var authState = await AuthenticationState;
                    var user = authState?.User;
                    if (user is not null)
                    {
                        if (user.IsInRole("Survey Validator") || user.IsInRole("Administrator"))
                        {
                            Surveys = await infrastructureService.GetAllBridgeSurveys();

                            var userId = authState.GetUserId();
                            if (userId is not null)
                            {
                                var teams = await maintenanceJobsService.GetMaintenanceTeamForUser(userId);
                                UserMaintenanceTeamIds = teams
                                    .Where(t => t is not null)
                                    .Select(t => t!.Id)
                                    .ToHashSet();
                            }
                        }
                        else
                        {
                            //load only surveys for the user
                            var userId = authState.GetUserId();
                            if (userId is not null)
                            {
                                Surveys = await infrastructureService.GetBridgeSurveysForUser(userId);
                            }
                            else
                            {
                                Surveys = [];
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred getting survey data for user");
                ErrorMessage = localizer["General Error Message"];
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnFilterByMyAreaChanged(ChangeEventArgs e)
        {
            FilterByMyArea = e.Value is true;
            if (SurveysAwaitingValidationGrid is not null)
            {
                await SurveysAwaitingValidationGrid.RefreshDataAsync();
            }
        }

        private async Task<GridDataProviderResult<BridgeSurvey>> SurveysAwaitingValidationDataProvider(GridDataProviderRequest<BridgeSurvey> request)
        {
            var surveysAwaitingValidation = Surveys.Where(s => s.Status is SurveyStatus.Completed);

            if (FilterByMyArea && UserMaintenanceTeamIds.Count > 0)
            {
                surveysAwaitingValidation = surveysAwaitingValidation
                    .Where(s => s.Infrastructure?.MaintenanceTeamId is not null
                             && UserMaintenanceTeamIds.Contains(s.Infrastructure.MaintenanceTeamId.Value));
            }

            var result = await Task.FromResult(request.ApplyTo(surveysAwaitingValidation));
            return result;
        }

        private async Task<GridDataProviderResult<BridgeSurvey>> IncompleteSurveysDataProvider(GridDataProviderRequest<BridgeSurvey> request)
        {
            var IncompleteSurveys = Surveys.Where(s => s.Status is SurveyStatus.Incomplete);
            var result = await Task.FromResult(request.ApplyTo(IncompleteSurveys));
            return result;
        }

        private async Task<GridDataProviderResult<BridgeSurvey>> FinishedSurveysDataProvider(GridDataProviderRequest<BridgeSurvey> request)
        {
            var FinishedSurveys = Surveys.Where(s => s.Status is SurveyStatus.Rejected or SurveyStatus.Verified);
            var result = await Task.FromResult(request.ApplyTo(FinishedSurveys));
            return result;
        }

        private void OnRowClick(GridRowEventArgs<BridgeSurvey> args) => navigationManager.NavigateTo($"surveys/bridge/{args.Item.Id}/details");
    }
}