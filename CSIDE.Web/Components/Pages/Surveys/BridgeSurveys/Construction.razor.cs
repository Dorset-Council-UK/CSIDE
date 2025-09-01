using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using CSIDE.Web.Services;
using Humanizer;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Construction(
        IInfrastructureService infrastructureService,
        NavigationManager navigationManager,
        ILogger<Construction> logger,
        ISettingsService settingsService)
    {
        [Parameter]
        public int SurveyId { get; init; }
        [SupplyParameterFromQuery]
        public bool FromSummary { get; init; }
        private BridgeSurvey? Survey { get; set; }
        private ICollection<Material> Materials { get; set; } = [];

        public string? ErrorMessage { get; set; }
        public bool IsBusy { get; set; }

        private List<BreadcrumbItem>? NavItems;

        private bool ShowBeamTimbersSection;
        private bool ShowDeckingBoardsSection;
        private bool ShowHandrailTimbersSection;
        private bool ShowHandrailPostsTimbersSection;

        private FluentValidationValidator? fluentValidationValidator;
        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
                new BreadcrumbItem{ Text = localizer["Bridge Survey Construction Title"], IsCurrentPage = true },
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
            Materials = await infrastructureService.GetBridgeSurveyMaterialOptions();
            OnRadioChange();
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
                    navigationManager.NavigateTo($"surveys/bridge/{Survey.Id}/condition");
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

        public void OnRadioChange()
        {
            if (Survey is not null && Materials is not null)
            {
                ShowBeamTimbersSection = Survey.BeamMaterialId.HasValue && Materials.Single(m => m.Id == Survey.BeamMaterialId).IsWood;
                ShowDeckingBoardsSection = Survey.DeckingMaterialId.HasValue && Materials.Single(m => m.Id == Survey.DeckingMaterialId).IsWood;
                ShowHandrailTimbersSection = Survey.HandrailMaterialId.HasValue && Materials.Single(m => m.Id == Survey.HandrailMaterialId).IsWood;
                ShowHandrailPostsTimbersSection = Survey.HandrailPostsMaterialId.HasValue && Materials.Single(m => m.Id == Survey.HandrailPostsMaterialId).IsWood;
                if (!ShowBeamTimbersSection)
                {
                    Survey.BeamTimbersSize = null;
                    Survey.NumBeamTimbers = null;
                }
                if (!ShowDeckingBoardsSection)
                {
                    Survey.DeckingBoardsLength = null;
                    Survey.DeckingBoardsSize = null;
                    Survey.NumDeckingBoards = null;
                }
                if (!ShowHandrailTimbersSection)
                {
                    Survey.HandrailTimbersSize = null;
                    Survey.HandrailsInPlace = null;
                }
                if (!ShowHandrailPostsTimbersSection)
                {
                    Survey.HandrailPostsTimbersSize = null;
                    Survey.NumHandrailPostsTimbers = null;
                }
            }
            
        }
    }
}