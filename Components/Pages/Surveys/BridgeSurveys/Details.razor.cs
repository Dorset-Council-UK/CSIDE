using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Surveys;
using CSIDE.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Details(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        NavigationManager navigationManager,
        ISettingsService settingsService)
    {
        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        [Parameter]
        public int SurveyId { get; init; }

        private BridgeSurvey? Survey { get; set; }

        public string? ErrorMessage { get; set; }
        public bool IsBusy { get; set; }

        private List<BreadcrumbItem>? NavItems;
        private string[] AuditLogSectionNames => [
            nameof(BridgeSurvey)
        ];

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

            if(Survey is null)
            {
                navigationManager.NavigateTo("/surveys/bridge/new");
                return;
            }
            if(Survey.Status is SurveyStatus.Incomplete)
            {
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/summary");
                return;
            }
            if (Survey.Status is SurveyStatus.Completed)
            {
                //check if user is validator
                if (authenticationState is not null)
                {
                    var authState = await authenticationState;
                    var user = authState?.User;
                    if (user is not null && user.IsInRole("Survey Validator"))
                    {
                        navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/validate");
                    }

                }
            }
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
    }
}