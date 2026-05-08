using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace CSIDE.Web.Components.DMMO
{
    public partial class CouncilDecisionEditForm
    {
        [Parameter, EditorRequired]
        public DMMOCouncilDecision? CouncilDecision { get; set; }
        [Parameter, EditorRequired]
        public ICollection<CouncilDecisionType> CouncilDecisionTypes { get; set; }
        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter, EditorRequired]
        public EventCallback<DMMOCouncilDecision> OnSubmit { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnCancel { get; set; }

        private FluentValidationValidator? fluentValidationValidator;

        private async Task HandleSubmit()
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(CouncilDecision);
            }
        }

        public async Task<bool> ValidateAsync()
        {
            return await fluentValidationValidator!.ValidateAsync();
        }

        private void UpdateDateSealedProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => CouncilDecision!.Date = date);
        }

        private void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate?> updateProperty)
        {
            if (CouncilDecision is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    updateProperty(parseResult.Value);
                }
                catch (Exception)
                {
                    // Problem parsing date, don't update
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
