using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Surveys;
using CSIDE.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace CSIDE.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Summary(
        IDbContextFactory<ApplicationDbContext> contextFactory, 
        NavigationManager navigationManager, 
        ILogger<Summary> logger,
        IGovNotifyEmailSender emailSender,
        IClock clock)
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

        protected override async Task OnParametersSetAsync()
        {
            //get survey
            using var context = contextFactory.CreateDbContext();
            Survey = await  context.BridgeSurveys
                .FindAsync(SurveyId);

            if(Survey is null)
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
                using var context = contextFactory.CreateDbContext();
                //get the existing job to enable the smarter change tracker.
                //Without this, all properties are identified as tracked, since
                //the DbContext is different from when the entity was queried
                var existingSurvey = await context.BridgeSurveys.IgnoreAutoIncludes().Where(s => s.Id == SurveyId).FirstAsync() ?? throw new Exception($"Survey being edited (ID: {SurveyId}) was not found prior to updating");

                context.Entry(existingSurvey).CurrentValues.SetValues(Survey);
                await context.SaveChangesAsync();
                navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/complete");
                await emailSender.SendNewBridgeSurveyNotification(Survey, navigationManager);
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