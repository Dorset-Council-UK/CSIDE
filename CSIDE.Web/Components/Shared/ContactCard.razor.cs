using BlazorBootstrap;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.Shared
{
    public partial class ContactCard(ISharedDataService sharedDataService, IJSRuntime JS, ILogger<ContactCard> logger)
    {
        [Parameter]
        public required Contact Contact { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }
        [Parameter]
        public EventCallback OnRefresh { get; set; }

        public bool IsBusy { get; set; }

        private string? ErrorMessage { get; set; }
        private Modal EditContactModal = default!;
        private IReadOnlyCollection<ContactType> ContactTypes { get; set; } = [];
        private Contact? WorkingContact { get; set; }
        private bool ContactHasBeenEdited { get; set; } = false;
        private ContactEditForm? EditContactForm;

        protected override async Task OnParametersSetAsync()
        {
            ContactTypes = await sharedDataService.GetContactTypeOptions();
        }

        private async Task SubmitFormAsync()
        {
            if (IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await EditContactForm!.ValidateAsync())
            {
                IsBusy = true;

                try
                {
                    if (WorkingContact is not null)
                    {
                        // Update the service with working copy
                        await sharedDataService.UpdateContact(WorkingContact);

                        // Only update the original Contact after successful save
                        Contact.Name = WorkingContact.Name;
                        Contact.Email = WorkingContact.Email;
                        Contact.PrimaryContactNo = WorkingContact.PrimaryContactNo;
                        Contact.SecondaryContactNo = WorkingContact.SecondaryContactNo;
                        Contact.OrganisationName = WorkingContact.OrganisationName;
                        Contact.ContactTypeId = WorkingContact.ContactTypeId;
                        Contact.ContactType = WorkingContact.ContactType;

                        ContactHasBeenEdited = true;
                        await HideEditContactModal();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred updating a contact");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task ShowEditContactModal()
        {
            // Create working copy for editing
            WorkingContact = CloneContact(Contact);
            await EditContactModal.ShowAsync();
        }
        private async Task HideEditContactModal()
        {
            //reset changes
            await EditContactModal.HideAsync();
        }

        private async Task DeleteContact(int ContactId)
        {
            IsBusy = true;
            try
            {
                bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Contact Confirmation"].Value);
                if (ConfirmDelete)
                {
                    await sharedDataService.DeleteContact(ContactId);
                    await RefreshComponent();
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred deleting a contact");
                ErrorMessage = localizer["Delete Error Message"];
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshComponent()
        {
            if (OnRefresh.HasDelegate)
            {
                await OnRefresh.InvokeAsync();
            }
        }

        private async Task OnModalHidingAsync()
        {
            // No need to reset anything - original Contact was never modified
            WorkingContact = null;
        }

        private Contact CloneContact(Contact original)
        {
            return new Contact
            {
                Id = original.Id,
                Name = original.Name,
                Email = original.Email,
                PrimaryContactNo = original.PrimaryContactNo,
                SecondaryContactNo = original.SecondaryContactNo,
                OrganisationName = original.OrganisationName,
                ContactTypeId = original.ContactTypeId,
                ContactType = original.ContactType,
                JobContact = original.JobContact,
                DMMOContact = original.DMMOContact,
                LandownerDepositContact = original.LandownerDepositContact
            };
        }
    }
}
