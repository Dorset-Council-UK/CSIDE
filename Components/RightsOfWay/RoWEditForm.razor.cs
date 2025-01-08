using Blazored.FluentValidation;
using CSIDE.Data.Models.RightsOfWay;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace CSIDE.Components.RightsOfWay
{
    public partial class RoWEditForm
    {
        [Parameter]
        public Data.Models.RightsOfWay.Route? Route { get; set; }
        [Parameter]
        public LegalStatus[]? LegalStatuses { get; set; }
        [Parameter]
        public OperationalStatus[]? OperationalStatuses { get; set; }
        [Parameter]
        public Data.Models.Maintenance.Team[]? MaintenanceTeams { get; set; }
        [Parameter]
        public RouteType[]? RouteTypes { get; set; }
        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool ClosureDatesShown { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter]
        public EventCallback OnSubmit { get; set; }
        [Parameter]
        public EventCallback OnCancel { get; set; }

        private FluentValidationValidator? fluentValidationValidator;

        protected override void OnAfterRender(bool firstRender)
        {
            if (Route is not null)
            {
                ShowOrHideClosureDates(Route.OperationalStatusId);
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
            if (!IsEdit)
            {
                return await fluentValidationValidator!.ValidateAsync(opts => opts.IncludeAllRuleSets());
            }
            return await fluentValidationValidator!.ValidateAsync();
        }

        private void UpdateClosureStartDateProperty(ChangeEventArgs eventArgs)
        {
            if (Route is not null && eventArgs.Value is not null)
            {
                try { 
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    Route.ClosureStartDate = parseResult.Value;
                }
                catch (Exception)
                {
                    //problem parsing date, don't update
                }
            }
        }

        private void UpdateClosureEndDateProperty(ChangeEventArgs eventArgs)
        {

            if (Route is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    Route.ClosureEndDate = parseResult.Value;
                }
                catch (Exception)
                { 
                    //problem parsing date, don't update
                }
            }
        }

        private void ShowOrHideClosureDates(ChangeEventArgs eventArgs)
        {
            if (Route is not null && int.TryParse(eventArgs.Value?.ToString(), CultureInfo.InvariantCulture, out int NewOperationalStatusId))
            {
                ShowOrHideClosureDates(NewOperationalStatusId);
            }
        }

        private void ShowOrHideClosureDates(int NewOperationalStatusId)
        {
            if (Route is not null)
            {
                var status = OperationalStatuses!.Where(s => s.Id == NewOperationalStatusId).FirstOrDefault();
                ClosureDatesShown = status != null && status.IsClosed;
                Route.OperationalStatusId = NewOperationalStatusId;
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
