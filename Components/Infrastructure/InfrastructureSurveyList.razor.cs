using CSIDE.Data;
using CSIDE.Data.Models.Surveys;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Infrastructure
{
    public partial class InfrastructureSurveyList(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<InfrastructureMediaList> logger)
    {
        [Parameter, EditorRequired]
        public required int InfrastructureItemId { get; set; }

        private List<BridgeSurvey>? Surveys { get; set; } = null;
        public bool IsLoading { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                Surveys = await context.BridgeSurveys.Where(s => s.InfrastructureItemId == InfrastructureItemId && s.Status == SurveyStatus.Verified).ToListAsync();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred fetching bridge surveys for infrastructure item {InfraItemId}", InfrastructureItemId);
            }
            finally
            {
                IsLoading = false;
            }
            await base.OnInitializedAsync();
        }
    }
}