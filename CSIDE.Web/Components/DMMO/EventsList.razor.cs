using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using CSIDE.Web.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NodaTime;
using System.Security.Claims;

namespace CSIDE.Web.Components.DMMO
{
    public partial class EventsList(IDMMOService dmmoService, ILogger<EventsList> logger)
    {
        [Parameter]
        public DMMOApplication? DMMOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
        private FluentValidationValidator? newEventValidator;

        private bool IsBusy { get; set; }
        private DMMOEvent? NewEvent { get; set; }
        private string? ErrorMessage { get; set; }

        protected override void OnParametersSet()
        {
            NewEvent = new DMMOEvent()
            {
                DMMOApplicationId = DMMOApplication!.Id,
                EventText = string.Empty,
                EventDate = LocalDate.FromDateTime(DateTime.Now),
            };

        }

        private async Task SubmitFormAsync()
        {
            if (IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await newEventValidator!.ValidateAsync())
            {
                IsBusy = true;

                try
                {
                    if (NewEvent is not null)
                    {

                        if (AuthenticationStateTask != null)
                        {
                            var authState = await AuthenticationStateTask;
                            NewEvent.AuthorId = authState.User.UserId;
                            NewEvent.AuthorName = authState.User.DisplayName;
                        }
                        await dmmoService.AddEventToDMMO(NewEvent);
                        await RefreshComponent();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating an avent");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void UpdateEventDateProperty(ChangeEventArgs eventArgs)
        {
            UpdateDateProperty(eventArgs, date => NewEvent!.EventDate = date);
        }

        private void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate> updateProperty)
        {
            if (DMMOApplication is null)
            {
                return;
            }

            DateInputHelper.UpdateDateProperty(eventArgs, updateProperty);
        }

        private async Task RefreshComponent()
        {
            if (DMMOApplication is not null)
            {
                NewEvent = new()
                {
                    DMMOApplicationId = DMMOApplication.Id,
                    EventText = string.Empty,
                    EventDate = LocalDate.FromDateTime(DateTime.Now),
                };
                DMMOApplication = await dmmoService.GetDMMOApplicationById(DMMOApplication.Id);
                StateHasChanged();
            }
        }
    }
}
