using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Surveys;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CSIDE.Web.Components.Pages.Surveys
{
    public partial class List(IDbContextFactory<ApplicationDbContext> contextFactory,
                              ILogger<List> logger,
                              NavigationManager navigationManager)
    {
        private List<BreadcrumbItem>? NavItems;
        public string? ErrorMessage { get; set; }
        public bool IsBusy { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }
        private List<BridgeSurvey> Surveys { get; set; } = [];
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
                using var context = contextFactory.CreateDbContext();
                if (AuthenticationState is not null)
                {
                    var authState = await AuthenticationState;
                    var user = authState?.User;
                    if (user is not null)
                    {
                        if (user.IsInRole("Survey Validator") || user.IsInRole("Administrator"))
                        {
                            //load all surveys
                            //TODO - Update logic to understand users validation area
                            Surveys = await context.BridgeSurveys.ToListAsync();
                        }
                        else
                        {
                            //load only surveys for the user
                            var userId = authState.GetUserId();
                            if (userId is not null)
                            {
                                Surveys = await context.BridgeSurveys
                                    .Where(s => s.SurveyorId == userId)
                                    .ToListAsync();
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

        private async Task<GridDataProviderResult<BridgeSurvey>> SurveysAwaitingValidationDataProvider(GridDataProviderRequest<BridgeSurvey> request)
        {
            var SurveysAwaitingValidation = Surveys.Where(s => s.Status is SurveyStatus.Completed);
            var result = await Task.FromResult(request.ApplyTo(SurveysAwaitingValidation));
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