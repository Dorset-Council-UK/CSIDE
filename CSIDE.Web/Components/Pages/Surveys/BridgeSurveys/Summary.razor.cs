using BlazorBootstrap;
using CSIDE.Data.Models.Surveys;
using CSIDE.Web.Services;
using CSIDE.Data.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Summary(
        IInfrastructureService infrastructureService, 
        NavigationManager navigationManager, 
        ILogger<Summary> logger,
        IGovNotifyEmailSender emailSender,
        IClock clock,
        ISettingsService settingsService)
    {
    
        [Parameter]
        public int SurveyId { get; init; }

        private BridgeSurvey? Survey { get; set; }

        public string? ErrorMessage { get; set; }
        public bool IsBusy { get; set; }

        private List<BreadcrumbItem>? NavItems;


        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
                new BreadcrumbItem{ Text = localizer["Bridge Survey Summary Title"], IsCurrentPage = true },
            ];
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
        protected override async Task OnParametersSetAsync()
        {
            //get survey
            Survey = await infrastructureService.GetBridgeSurveyById(SurveyId);

            if (Survey is null)
            {
                navigationManager.NavigateTo("/surveys/bridge/new");
            }

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
                Survey.Status = SurveyStatus.Completed;
                Survey.EndDate = clock.GetCurrentInstant();
                await infrastructureService.UpdateBridgeSurvey(SurveyId, Survey);
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/complete");
                string validationUrl = navigationManager.ToAbsoluteUri($"surveys/bridge/{Survey.Id}/validate").ToString();
                await emailSender.SendNewBridgeSurveyNotification(Survey, validationUrl);
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