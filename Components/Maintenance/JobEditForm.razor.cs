using Blazored.FluentValidation;
using CSIDE.Data.Models.Maintenance;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace CSIDE.Components.Maintenance
{
    public partial class JobEditForm
    {
        [Parameter]
        public Job? Job { get; set; }
        [Parameter]
        public JobStatus[]? JobStatuses { get; set; }
        [Parameter]
        public JobPriority[]? JobPriorities { get; set; }
        [Parameter]
        public Team[]? MaintenanceTeams { get; set; }
        [Parameter]
        public ProblemType[]? ProblemTypes { get; set; }
        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool CompleteDateShown { get; set; }
        [Parameter]
        public bool DuplicateOfShown { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter]
        public EventCallback OnSubmit { get; set; }
        [Parameter]
        public EventCallback OnCancel { get; set; }
        [Parameter]
        public IList<int> SelectedProblemTypes { get; set; } = [];

        private FluentValidationValidator? fluentValidationValidator;

        protected override void OnAfterRender(bool firstRender)
        {
            if (Job is not null && Job.JobStatusId.HasValue)
            {
                ShowOrHideCompletionDate(Job.JobStatusId.Value);
                ShowOrHideDuplicateOf(Job.JobStatusId.Value);
                StateHasChanged();
            }
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

        private void UpdateCompletionDateProperty(ChangeEventArgs eventArgs)
        {
            if (Job is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    Job.CompletionDate = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void ShowOrHideCompletionDate(ChangeEventArgs eventArgs)
        {
            if (Job is not null && int.TryParse(eventArgs.Value?.ToString(), CultureInfo.InvariantCulture, out int NewJobStatusId))
            {
                ShowOrHideCompletionDate(NewJobStatusId);
            }
        }
        private void ShowOrHideCompletionDate(int NewJobStatusId)
        {
            if (Job is not null)
            {
                CompleteDateShown = JobStatuses!.Where(s => s.Id == NewJobStatusId).First().IsComplete;
                Job.JobStatusId = NewJobStatusId;
            }
        }

        private void ShowOrHideDuplicateOf(int NewJobStatusId)
        {
            if (Job is not null)
            {
                DuplicateOfShown = JobStatuses!.Where(s => s.Id == NewJobStatusId).First().IsDuplicate;
                Job.JobStatusId = NewJobStatusId;
            }
        }

        private async Task HandleCancel()
        {
            if (OnCancel.HasDelegate)
            {
                await OnCancel.InvokeAsync();
            }
        }

        private void ProblemTypeChanged(ProblemType ProblemType, ChangeEventArgs eventArgs)
        {

            if (Convert.ToBoolean(eventArgs.Value))
            {
                if (!SelectedProblemTypes.ToList().Contains(ProblemType.Id))
                {
                    SelectedProblemTypes.Add(ProblemType.Id);
                }
            }
            else
            {
                if (SelectedProblemTypes.ToList().Contains(ProblemType.Id))
                {
                    SelectedProblemTypes.Remove(ProblemType.Id);
                }
            }
        }
    }
}
