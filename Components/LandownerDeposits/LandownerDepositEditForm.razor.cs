using Blazored.FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Components.LandownerDeposits
{
    public partial class LandownerDepositEditForm(IUserService userService)
    {
        [Parameter]
        public LandownerDeposit? LandownerDeposit { get; set; }
        [Parameter]
        public LandownerDepositTypeName[]? LandownerDepositTypeNames {  get; set; }

        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public bool IsEdit { get; set; }
        [Parameter]
        public EventCallback OnSubmit { get; set; }
        [Parameter]
        public EventCallback OnCancel { get; set; }
        [Parameter]
        public IList<int> SelectedLandownerDepositTypes { get; set; } = [];
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
            return await fluentValidationValidator!.ValidateAsync();
        }

        private async Task HandleCancel()
        {
            if (OnCancel.HasDelegate)
            {
                await OnCancel.InvokeAsync();
            }
        }

        private void LandownerDepositTypeChanged(LandownerDepositTypeName landownerDepositTypeName, ChangeEventArgs eventArgs)
        {

            if (Convert.ToBoolean(eventArgs.Value))
            {
                if (!SelectedLandownerDepositTypes.ToList().Contains(landownerDepositTypeName.Id))
                {
                    SelectedLandownerDepositTypes.Add(landownerDepositTypeName.Id);
                }
            }
            else
            {
                if (SelectedLandownerDepositTypes.ToList().Contains(landownerDepositTypeName.Id))
                {
                    SelectedLandownerDepositTypes.Remove(landownerDepositTypeName.Id);
                }
            }
        }

        private void UpdateReceivedDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.ReceivedDate = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateAcknowledgedDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.DateAcknowledged = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateChequePaidDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.ChequePaidInDate = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateNoticeDraftedDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.NoticeDrafted = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateWebsiteNoticeDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.WebsiteNoticePublished = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateEmailNoticeDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.EmailNoticeSent = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateOnsiteNoticeDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.OnsiteNoticeErected = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateWebsiteEntryDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.WebsiteEntryAdded = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private void UpdateArchiveDateProperty(ChangeEventArgs eventArgs)
        {
            if (LandownerDeposit is not null && eventArgs.Value is not null)
            {
                try
                {
                    var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                    var parseResult = pattern.Parse(eventArgs.Value.ToString()!);
                    LandownerDeposit.SentToArchive = parseResult.Value;
                }
                catch (Exception)
                {
                    //unparsable date, don't update property
                }
            }
        }

        private async Task<IList<string>> GetCaseOfficerSuggestions()
        {
            var users = await userService.GetUsersInRole("RoW Officer");
            if (users is not null)
            {
                return [.. users.Select(u => u.DisplayName ?? string.Empty).OrderBy(u => u)];
            }
            return [];
        }
    }
}
