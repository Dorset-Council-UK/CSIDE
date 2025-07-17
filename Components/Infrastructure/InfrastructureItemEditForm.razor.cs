using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Infrastructure
{
    public partial class InfrastructureItemEditForm(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        [Parameter]
        public InfrastructureItem? InfrastructureItem { get; set; }
        [Parameter]
        public InfrastructureType[]? InfrastructureTypes { get; set; }

        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter]
        public EventCallback OnSubmit { get; set; }
        [Parameter]
        public EventCallback OnCancel { get; set; }

        private Data.Models.Surveys.Material[]? BridgeMaterials { get; set; }
        private Data.Models.Surveys.Condition[]? BridgeConditions { get; set; }
        private bool ShowBeamTimbersSection { get; set; }
        private bool ShowDeckingBoardsSection { get; set; }
        private bool ShowHandrailTimbersSection { get; set; }
        private bool ShowHandrailPostsTimbersSection { get; set; }
        private bool ShowBridgeDetailsEditingSection { get; set; }

        private FluentValidationValidator? fluentValidationValidator;

        protected override async Task OnParametersSetAsync()
        {
            using var context = contextFactory.CreateDbContext();
            BridgeMaterials = await context.Materials.ToArrayAsync();
            BridgeConditions = await context.Conditions.ToArrayAsync();
            OnRadioChange();
        }

        private async Task SubmitFormAsync()
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync();
            }
        }

        public async Task<bool> ValidateAsync()
        {
            return await fluentValidationValidator!.ValidateAsync();
        }

        private void UpdateInstallationDateProperty(ChangeEventArgs eventArgs)
        {
            if (InfrastructureItem is not null && eventArgs.Value is not null)
            {
                var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                InfrastructureItem.InstallationDate = parseResult.Value;
            }
        }

        public void OnRadioChange()
        {
            if (InfrastructureItem is not null)
            {
                InfrastructureItem.InfrastructureType = InfrastructureTypes?.Where(i => i.Id == InfrastructureItem.InfrastructureTypeId).First();
                if (InfrastructureItem.InfrastructureType is not null && InfrastructureItem.InfrastructureType.IsBridge)
                {
                    ShowBridgeDetailsEditingSection = true;
                    if (InfrastructureItem.BridgeDetails is not null && BridgeMaterials is not null)
                    {
                        ShowBeamTimbersSection = InfrastructureItem.BridgeDetails.BeamMaterialId.HasValue && BridgeMaterials.Where(m => m.Id == InfrastructureItem.BridgeDetails.BeamMaterialId).Single().IsWood;
                        ShowDeckingBoardsSection = InfrastructureItem.BridgeDetails.DeckingMaterialId.HasValue && BridgeMaterials.Where(m => m.Id == InfrastructureItem.BridgeDetails.DeckingMaterialId).Single().IsWood;
                        ShowHandrailTimbersSection = InfrastructureItem.BridgeDetails.HandrailMaterialId.HasValue && BridgeMaterials.Where(m => m.Id == InfrastructureItem.BridgeDetails.HandrailMaterialId).Single().IsWood;
                        ShowHandrailPostsTimbersSection = InfrastructureItem.BridgeDetails.HandrailPostsMaterialId.HasValue && BridgeMaterials.Where(m => m.Id == InfrastructureItem.BridgeDetails.HandrailPostsMaterialId).Single().IsWood;
                        if (!ShowBeamTimbersSection)
                        {
                            InfrastructureItem.BridgeDetails.BeamTimbersSize = null;
                            InfrastructureItem.BridgeDetails.NumBeamTimbers = null;
                        }
                        if (!ShowDeckingBoardsSection)
                        {
                            InfrastructureItem.BridgeDetails.DeckingBoardsLength = null;
                            InfrastructureItem.BridgeDetails.DeckingBoardsSize = null;
                            InfrastructureItem.BridgeDetails.NumDeckingBoards = null;
                        }
                        if (!ShowHandrailTimbersSection)
                        {
                            InfrastructureItem.BridgeDetails.HandrailTimbersSize = null;
                            InfrastructureItem.BridgeDetails.HandrailsInPlace = null;
                        }
                        if (!ShowHandrailPostsTimbersSection)
                        {
                            InfrastructureItem.BridgeDetails.HandrailPostsTimbersSize = null;
                            InfrastructureItem.BridgeDetails.NumHandrailPostsTimbers = null;
                        }
                    }
                }
                else
                {
                    ShowBridgeDetailsEditingSection = false;
                }
            }
        }

        private async Task HandleCancel()
        {
            if (OnCancel.HasDelegate)
            {
                await OnCancel.InvokeAsync();
            }
        }
    }
}
