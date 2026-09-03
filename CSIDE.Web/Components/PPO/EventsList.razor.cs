using Blazored.FluentValidation;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Services;
using CSIDE.Web.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NodaTime;
using System.Security.Claims;

namespace CSIDE.Web.Components.PPO
{
    public partial class EventsList(IPPOService ppoService, ILogger<EventsList> logger)
    {
        [Parameter]
        public PPOApplication? PPOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
        private FluentValidationValidator? newEventValidator;

        private bool IsBusy { get; set; }
        private PPOEvent? NewEvent { get; set; }
        private string? ErrorMessage { get; set; }

        protected override void OnParametersSet()
        {
            NewEvent = new PPOEvent()
            {
                PPOApplicationId = PPOApplication!.Id,
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

                        await ppoService.AddEventToPPO(NewEvent);
                        await RefreshComponent();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating an event");
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
            if (PPOApplication is null)
            {
                return;
            }

            DateInputHelper.UpdateDateProperty(eventArgs, updateProperty);
        }

        private async Task RefreshComponent()
        {
            if (PPOApplication is not null)
            {
                NewEvent = new()
                {
                    PPOApplicationId = PPOApplication.Id,
                    EventText = string.Empty,
                    EventDate = LocalDate.FromDateTime(DateTime.Now),
                };
                PPOApplication = await ppoService.GetPPOApplicationById(PPOApplication.Id);
                StateHasChanged();
            }
        }
    }
}
