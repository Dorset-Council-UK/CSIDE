using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Infrastructure
{
    public partial class InfrastructureSurveyList(IInfrastructureService infrastructureService, ILogger<InfrastructureMediaList> logger)
    {
        [Parameter, EditorRequired]
        public required int InfrastructureItemId { get; set; }

        private ICollection<BridgeSurvey>? Surveys { get; set; } = null;
        public bool IsLoading { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            try
            {
                Surveys = await infrastructureService.GetValidatedBridgeSurveysByInfrastructureItemId(InfrastructureItemId);
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