using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using CSIDE.Data;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Models.PPO;
using NodaTime;

namespace CSIDE.Components.PPO
{
    public partial class EventsList(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<EventsList> logger)
    {
        [Parameter]
        public Application? PPOApplication { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        private FluentValidationValidator? newEventValidator;

        private bool IsBusy { get; set; }
        private PPOEvent? NewEvent { get; set; }
        private string? ErrorMessage { get; set; }

        protected override void OnParametersSet()
        {
            NewEvent = new PPOEvent()
            {
                ApplicationId = PPOApplication!.Id,
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
                        using var context = contextFactory.CreateDbContext();

                        var user = (await AuthenticationStateTask).User;
                        NewEvent.AuthorId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
                        NewEvent.AuthorName = user.Identity?.Name;

                        context.PPOEvents.Add(NewEvent);
                        await context.SaveChangesAsync();
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

        private async Task RefreshComponent()
        {
            if (PPOApplication is not null)
            {
                NewEvent = new()
                {
                    ApplicationId = PPOApplication.Id,
                    EventText = string.Empty,
                    EventDate = LocalDate.FromDateTime(DateTime.Now),
                };
                using var context = contextFactory.CreateDbContext();
                PPOApplication = await context.PPOApplication.FindAsync([PPOApplication.Id]);
                StateHasChanged();
            }
        }
    }
}
