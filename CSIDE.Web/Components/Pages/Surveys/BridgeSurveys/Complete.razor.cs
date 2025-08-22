using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Surveys;
using CSIDE.Web.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Complete(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        NavigationManager navigationManager,
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
                new BreadcrumbItem{ Text = localizer["Survey Complete Title"], IsCurrentPage = true },
            ];
        }

        protected override async Task OnParametersSetAsync()
        {
            //get survey
            using var context = contextFactory.CreateDbContext();
            Survey = await context.BridgeSurveys
                .FindAsync(SurveyId);

            if (Survey is null)
            {
                navigationManager.NavigateTo("/surveys/bridge/new");
                return;
            }
            if (Survey.Status is SurveyStatus.Incomplete)
            {
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/summary");
                return;
            }
            if (Survey.Status is not SurveyStatus.Completed)
            {
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/details");
                return;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Survey is not null)
            {
                //add to recent work store
                await settingsService.AddRecentWork($"{IDPrefixOptions.Value.Infrastructure}/{Survey.InfrastructureItemId}", "Survey", Survey.Status.Humanize() , $"surveys/bridge/{Survey.Id}/details");
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}