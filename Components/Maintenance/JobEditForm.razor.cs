using Blazored.FluentValidation;
using CSIDE.Data.Models.Maintenance;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Components.Maintenance;

public partial class JobEditForm()
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
    public bool DuplicateOfShown { get; set; }
    [Parameter]
    public bool IsEdit { get; set; }
    [Parameter]
    public EventCallback OnSubmit { get; set; }
    [Parameter]
    public EventCallback OnCancel { get; set; }
    [Parameter]
    public IList<int> SelectedProblemTypes { get; set; } = [];

    public bool CompleteDateShown { get; set; }
    private FluentValidationValidator? fluentValidationValidator;

    protected override void OnParametersSet()
    {
        if(Job is not null)
        {
            AfterStatusIdChanged();
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
        if (fluentValidationValidator is null)
        {
            return false;
        }
        return await fluentValidationValidator.ValidateAsync();
    }

    private void UpdateCompletionDateProperty(ChangeEventArgs eventArgs)
    {
        if (Job is not null && eventArgs.Value is not null)
        {
            try
            {
                var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                var parseResult = pattern.Parse(eventArgs.Value.ToString());
                Job.CompletionDate = parseResult.Value;
            }
            catch (Exception)
            {
                //unparsable date, don't update property
            }
        }
    }

    /// <summary>
    /// Triggered after JobStatusId changes. When it has a different value.
    /// </summary>
    private void AfterStatusIdChanged()
    {
        if (Job is null || JobStatuses is null)
        {
            return;
        }

        var jobStatus = JobStatuses.FirstOrDefault(s => s.Id == Job.JobStatusId);
        if (jobStatus is null)
        {
            CompleteDateShown = false;
            DuplicateOfShown = false;
            return;
        }

        CompleteDateShown = jobStatus.IsComplete;
        DuplicateOfShown = jobStatus.IsDuplicate;
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
            if (!SelectedProblemTypes.Contains(ProblemType.Id))
            {
                SelectedProblemTypes.Add(ProblemType.Id);
            }
        }
        else
        {
            if (SelectedProblemTypes.Contains(ProblemType.Id))
            {
                SelectedProblemTypes.Remove(ProblemType.Id);
            }
        }
    }
}
