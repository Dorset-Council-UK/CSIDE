using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Surveys;
using CSIDE.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Condition(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        NavigationManager navigationManager,
        ILogger<Condition> logger,
        ISettingsService settingsService)
    {
        [Parameter]
        public int SurveyId { get; init; }
        [SupplyParameterFromQuery]
        public bool FromSummary { get; init; }
        private BridgeSurvey? Survey { get; set; }
        private Data.Models.Surveys.Condition[]? Conditions { get; set; }

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
            using var context = contextFactory.CreateDbContext();
            Survey = await context.BridgeSurveys.IgnoreAutoIncludes().Where(s => s.Id == SurveyId).FirstOrDefaultAsync();

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
            Conditions = await context.Conditions.OrderBy(x => x.Name).ToArrayAsync();
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

                context.Entry(existingSurvey).CurrentValues.SetValues(Survey);

                await context.SaveChangesAsync();
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