using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace CSIDE.Web.Components.DMMO
{
    public partial class DMMOEditForm(IUserService userService)
    {
        [Parameter, EditorRequired]
        public DMMOApplication? DMMOApplication { get; set; }
        [Parameter, EditorRequired]
        public ICollection<ApplicationCaseStatus> CaseStatuses { get; set; }
        [Parameter, EditorRequired]
        public ICollection<ApplicationType> ApplicationTypes { get; set; }
        [Parameter, EditorRequired]
        public ICollection<ApplicationClaimedStatus> ClaimedStatuses { get; set; }
        [Parameter, EditorRequired]
        public ICollection<ApplicationDirectionOfSecState> DirectionsOfSecState { get; set; }
        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnSubmit { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnCancel { get; set; }
        public IList<string> CaseOfficerSuggestions = [];

        private FluentValidationValidator? fluentValidationValidator;

        protected override async Task OnInitializedAsync()
        {
            CaseOfficerSuggestions = await GetCaseOfficerSuggestions();
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
            if (!IsEdit)
            {
                return await fluentValidationValidator!.ValidateAsync(opts => opts.IncludeAllRuleSets());
            }
            return await fluentValidationValidator!.ValidateAsync();
        }

        private void UpdateApplicationDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => DMMOApplication!.ApplicationDate = date);
        }

        private void UpdateDirectionOfSecStateDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => DMMOApplication!.DateOfDirectionOfSecState = date);
        }

        private void UpdateDeterminationDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => DMMOApplication!.DeterminationDate = date);
        }

        private void UpdateAppealDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => DMMOApplication!.AppealDate = date);
        }
        private void UpdateReceivedDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => DMMOApplication!.ReceivedDate = date);
        }

        private void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate?> updateProperty)
        {
            if (DMMOApplication is not null && eventArgs.Value is not null)
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

        private async Task<IList<string>> GetCaseOfficerSuggestions()
        {
            var users = await userService.GetUsersInRole("RoW Officer");
            if (users is not null)
            {
                return [.. users.Select(u => u.DisplayName ?? string.Empty).Order(StringComparer.OrdinalIgnoreCase)];
            }
            return [];
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
