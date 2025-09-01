using BlazorBootstrap;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using CSIDE.Web.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Condition(
        IInfrastructureService infrastructureService,
        NavigationManager navigationManager,
        ILogger<Condition> logger,
        ISettingsService settingsService)
    {
        [Parameter]
        public int SurveyId { get; init; }
        [SupplyParameterFromQuery]
        public bool FromSummary { get; init; }
        private BridgeSurvey? Survey { get; set; }
        private ICollection<Data.Models.Surveys.Condition> Conditions { get; set; } = [];

        public string? ErrorMessage { get; set; }
        public bool IsBusy { get; set; }

        private List<BreadcrumbItem>? NavItems;

        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
                new BreadcrumbItem{ Text = localizer["Bridge Survey Condition Title"], IsCurrentPage = true },
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
            if (Survey.Status is not SurveyStatus.Incomplete)
            {
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/details");
                return;
            }
            Conditions = await infrastructureService.GetBridgeSurveyConditionOptions();
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
               await infrastructureService.UpdateBridgeSurvey(SurveyId, Survey);
                if (FromSummary)
                {
                    navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/summary");
                }
                else
                {
                    navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/repairs-and-further-information");
                }
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