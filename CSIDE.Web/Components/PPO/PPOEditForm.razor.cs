using Blazored.FluentValidation;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace CSIDE.Web.Components.PPO
{
    public partial class PPOEditForm(IUserService userService)
    {
        [Parameter, EditorRequired]
        public PPOApplication? PPOApplication { get; set; }
        [Parameter, EditorRequired]
        public IReadOnlyCollection<ApplicationCaseStatus>? CaseStatuses { get; set; }
        [Parameter, EditorRequired]
        public IReadOnlyCollection<ApplicationLegislation>? Legislation { get; set; }
        [Parameter, EditorRequired]
        public IReadOnlyCollection<ApplicationIntent>? Intents { get; set; }
        [Parameter, EditorRequired]
        public IReadOnlyCollection<ApplicationPriority>? Priorities { get; set; }
        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnSubmit { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnCancel { get; set; }
        [Parameter]
        public IList<int> SelectedIntents { get; set; } = [];

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

        private void UpdateDirectionDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => PPOApplication!.DateOfDirection = date);
        }

        private void UpdateDeterminationDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => PPOApplication!.DeterminationDate = date);
        }

        private void UpdateInspectionCertificationDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => PPOApplication!.InspectionCertificationDate = date);
        }
        private void UpdateReceivedDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => PPOApplication!.ReceivedDate = date);
        }
        private void UpdateConfirmationPublishedDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => PPOApplication!.ConfirmationPublishedDate = date);
        }

        private void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate?> updateProperty)
        {
            if (PPOApplication is not null && eventArgs.Value is not null)
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
        private void IntentChanged(ApplicationIntent Intent, ChangeEventArgs eventArgs)
        {
            if (Convert.ToBoolean(eventArgs.Value))
            {
                if (!SelectedIntents.Contains(Intent.Id))
                {
                    SelectedIntents.Add(Intent.Id);
                }
            }
            else
            {
                if (SelectedIntents.Contains(Intent.Id))
                {
                    SelectedIntents.Remove(Intent.Id);
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

        private async Task<IList<string>> GetCaseOfficerSuggestions()
        {
            var users = await userService.GetUsersInRole("RoW Officer");
            if (users is not null)
            {
                return [.. users.Select(u => u.DisplayName ?? string.Empty).Order(StringComparer.OrdinalIgnoreCase)];
            }
            return [];
        }
    }
}
