using Blazored.FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Services;
using CSIDE.Web.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NodaTime;
using System.Security.Claims;

namespace CSIDE.Web.Components.LandownerDeposits
{
    public partial class EventsList(ILandownerDepositService landownerDepositService, ILogger<EventsList> logger)
    {
        [Parameter]
        public LandownerDeposit? LandownerDeposit { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
        private FluentValidationValidator? newEventValidator;

        private bool IsBusy { get; set; }
        private LandownerDepositEvent? NewEvent { get; set; }
        private string? ErrorMessage { get; set; }

        protected override void OnParametersSet()
        {
            NewEvent = new LandownerDepositEvent()
            {
                LandownerDepositId = LandownerDeposit!.Id,
                LandownerDepositSecondaryId = LandownerDeposit!.SecondaryId,
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

                        await landownerDepositService.AddEventToLandownerDeposit(NewEvent);
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
            if (LandownerDeposit is null)
            {
                return;
            }

            DateInputHelper.UpdateDateProperty(eventArgs, updateProperty);
        }

        private async Task RefreshComponent()
        {
            if (LandownerDeposit is not null)
            {
                NewEvent = new()
                {
                    LandownerDepositId = LandownerDeposit.Id,
                    LandownerDepositSecondaryId = LandownerDeposit!.SecondaryId,
                    EventText = string.Empty,
                    EventDate = LocalDate.FromDateTime(DateTime.Now),
                };
                LandownerDeposit = await landownerDepositService.GetLandownerDepositById(LandownerDeposit.Id, LandownerDeposit.SecondaryId);
                StateHasChanged();
            }
        }
    }
}
