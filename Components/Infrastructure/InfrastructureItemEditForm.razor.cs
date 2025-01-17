using Blazored.FluentValidation;
using CSIDE.Data.Models.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Components.Infrastructure
{
    public partial class InfrastructureItemEditForm
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

        private FluentValidationValidator? fluentValidationValidator;

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

        private void UpdateCompletionDateProperty(ChangeEventArgs eventArgs)
        {
            if (InfrastructureItem is not null && eventArgs.Value is not null)
            {
                var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                InfrastructureItem.InstallationDate = parseResult.Value;
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
