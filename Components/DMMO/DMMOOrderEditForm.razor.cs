using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using Microsoft.AspNetCore.Components;
using NodaTime;
using System.Globalization;

namespace CSIDE.Components.DMMO
{
    public partial class DMMOOrderEditForm
    {
        [Parameter, EditorRequired]
        public Order? Order { get; set; }
        [Parameter, EditorRequired]
        public OrderDecisionOfSecState[]? OrderDecisionsOfSecState { get; set; }
        [Parameter, EditorRequired]
        public OrderDeterminationProcess[]? OrderDeterminationProcesses { get; set; }
        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnSubmit { get; set; }
        [Parameter, EditorRequired]
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
            if (!IsEdit)
            {
                return await fluentValidationValidator!.ValidateAsync(opts => opts.IncludeAllRuleSets());
            }
            return await fluentValidationValidator!.ValidateAsync();
        }

        private void UpdateDateSealedProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => Order!.DateSealed = date);
        }

        private void UpdateDatePublishedProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => Order!.DatePublished = date);
        }

        private void UpdateObjectionsEndDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => Order!.ObjectionsEndDate = date);
        }
        private void UpdateConfirmedDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => Order!.DateConfirmed = date);
        }

        private void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate?> updateProperty)
        {
            if (Order is not null && eventArgs.Value is not null)
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
