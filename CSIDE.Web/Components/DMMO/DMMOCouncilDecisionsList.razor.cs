using BlazorBootstrap;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.DMMO
{
    public partial class DMMOCouncilDecisionsList(IJSRuntime JS, IDMMOService dmmoService)
    {
        [Parameter]
        public ICollection<DMMOCouncilDecision>? CouncilDecisions { get; set; }
        [Parameter]
        public int DMMOApplicationId { get; set; }
        [Parameter]
        public EventCallback<(DMMOCouncilDecision CouncilDecision, bool IsEdit)> OnSubmit { get; set; }
        [Parameter]
        public EventCallback OnRefresh { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = false;

        private DMMOCouncilDecision? NewCouncilDecision { get; set; }
        private ICollection<CouncilDecisionType> CouncilDecisionTypes { get; set; } = [];
        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }
        private CouncilDecisionEditForm? NewCouncilDecisionForm;
        private bool IsEdit { get; set; }

        private Modal AddCouncilDecisionModal = default!;

        protected override async Task OnParametersSetAsync()
        {
            NewCouncilDecision = new();
            CouncilDecisionTypes = await dmmoService.GetCouncilDecisionTypeOptions();
        }

        private async Task SubmitFormAsync()
        {
            if (IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await NewCouncilDecisionForm!.ValidateAsync())
            {
                IsBusy = true;
                if (OnSubmit.HasDelegate)
                {
                    await OnSubmit.InvokeAsync((NewCouncilDecision!, IsEdit));
                }
                IsBusy = false;
                await HideAddCouncilDecisionModal();
                await RefreshComponent();
            }
        }

        private async Task ShowAddCouncilDecisionModal()
        {
            IsEdit = false;
            NewCouncilDecision = new();
            await AddCouncilDecisionModal.ShowAsync();
        }

        private async Task ShowEditCouncilDecisionModal(DMMOCouncilDecision councilDecision)
        {
            IsEdit = true;
            NewCouncilDecision = new()
            {
                CouncilDecisionId = councilDecision.CouncilDecisionId,
                DMMOApplicationId = councilDecision.DMMOApplicationId,
                CouncilDecisionTypeId = councilDecision.CouncilDecisionTypeId,
                Date = councilDecision.Date,
                Notes = councilDecision.Notes,
            };
            await AddCouncilDecisionModal.ShowAsync();
        }

        private async Task HideAddCouncilDecisionModal()
        {
            await AddCouncilDecisionModal.HideAsync();
        }

        private async Task DeleteCouncilDecision(int ApplicationId, int CouncilDecisionId)
        {
            IsBusy = true;
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Council Decision Confirmation"].Value);
            if (ConfirmDelete)
            {
                await dmmoService.DeleteDMMOCouncilDecision(ApplicationId, CouncilDecisionId);
                await RefreshComponent();
                
            }
            IsBusy = false;
        }

        private async Task RefreshComponent()
        {
            CouncilDecisions = await dmmoService.GetCouncilDecisionsByApplicationId(DMMOApplicationId);
            StateHasChanged();
        }
    }

}