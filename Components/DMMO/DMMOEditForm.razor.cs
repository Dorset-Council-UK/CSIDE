using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using Microsoft.AspNetCore.Components;
using NodaTime;
using System.Globalization;

namespace CSIDE.Components.DMMO
{
    public partial class DMMOEditForm
    {
        [Parameter]
        public Application? DMMOApplication { get; set; }
        [Parameter]
        public ApplicationCaseStatus[]? CaseStatuses { get; set; }
        [Parameter]
        public ApplicationType[]? ApplicationTypes { get; set; }
        [Parameter]
        public ApplicationClaimedStatus[]? ClaimedStatuses { get; set; }
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
            if (!IsEdit)
            {
                return await fluentValidationValidator!.ValidateAsync(opts => opts.IncludeAllRuleSets());
            }
            return await fluentValidationValidator!.ValidateAsync();
        }

        private void UpdateApplicationDateProperty(ChangeEventArgs eventArgs)
        {

            if (DMMOApplication is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    DMMOApplication.ApplicationDate = parseResult.Value;
                }
                catch (Exception)
                { 
                    //problem parsing date, don't update
                }
            }
        }

        private void UpdateReceivedDateProperty(ChangeEventArgs eventArgs)
        {

            if (DMMOApplication is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    DMMOApplication.ReceivedDate = parseResult.Value;
                }
                catch (Exception)
                {
                    //problem parsing date, don't update
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
