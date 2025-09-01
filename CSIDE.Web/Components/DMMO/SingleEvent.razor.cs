using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NodaTime;

namespace CSIDE.Web.Components.DMMO
{
    public partial class SingleEvent(IDMMOService dmmoService, IJSRuntime JS, ILogger<SingleEvent> logger)
    {
        [Parameter]
        public required DMMOEvent Event { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }
        [Parameter]
        public EventCallback OnRefresh { get; set; }

        private FluentValidationValidator? editEventValidator;

        private bool IsBusy { get; set; }
        private bool IsEditing { get; set; }
        private string? ErrorMessage { get; set; }
        private string? OriginalValue { get; set; }

        protected override void OnParametersSet()
        {
            OriginalValue = Event.EventText;
        }

        private async Task DeleteEvent(int EventId)
        {
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Event Confirmation"].Value);
            if (ConfirmDelete)
            {
                await dmmoService.DeleteDMMOEvent(EventId);
                await RefreshComponent();
            }
        }

        private async Task EditEvent(DMMOEvent dmmoEvent)
        {
            if (IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await editEventValidator!.ValidateAsync())
            {
                IsBusy = true;

                try
                {
                    if (dmmoEvent is not null)
                    {
                        await dmmoService.UpdateDMMOEvent(Event.Id, dmmoEvent);
                        //refresh component by simply switching off editing mode. We don't need to refetch the data, its already there!
                        IsEditing = false;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred updating an event");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
        private void UpdateEventDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => Event!.EventDate = date);
        }

        private static void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate> updateProperty)
        {
            try
            {
                var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                var parseResult = pattern.Parse(eventArgs.Value?.ToString()!);
                updateProperty(parseResult.Value);
            }
            catch (Exception)
            {
                // Problem parsing date, don't update
            }
        }
        
        private void CancelUpdate()
        {
            IsEditing = false;
            //reset
            if (OriginalValue is not null)
            {
                Event.EventText = OriginalValue;
            }
        }

        private async Task RefreshComponent()
        {
            if (OnRefresh.HasDelegate)
            {
                await OnRefresh.InvokeAsync();
            }
        }
    }
}
