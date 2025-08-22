using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.Shared
{
    public partial class ContactCard(IDbContextFactory<ApplicationDbContext> contextFactory, IJSRuntime JS, ILogger<ContactCard> logger)
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
        private ContactType[]? ContactTypes { get; set; }

        private ContactEditForm? EditContactForm;

        protected override async Task OnParametersSetAsync()
        {
            using var context = contextFactory.CreateDbContext();
            ContactTypes = await context.ContactTypes.OrderBy(c => c.Id).ToArrayAsync();
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
                    if (Contact is not null)
                    {
                        using var context = contextFactory.CreateDbContext();
                        //clear the 'ContactType' navigation property, otherwise ef core uses that which hasn't changed
                        Contact.ContactType = null;

                        //get the existing job to enable the smarter change tracker.
                        //Without this, all properties are identified as tracked, since
                        //the DbContext is different from when the entity was queried
                        var existingContact = await context.Contacts.FindAsync(Contact.Id) ?? throw new Exception($"Contact being edited (ID: {Contact.Id}) was not found prior to updating");

                        context.Entry(existingContact).CurrentValues.SetValues(Contact);

                        await context.SaveChangesAsync();

                        await HideEditContactModal();
                        //refresh component
                        await RefreshComponent();
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
            await EditContactModal.ShowAsync();
        }
        private async Task HideEditContactModal()
        {
            await EditContactModal.HideAsync();
        }

        private async Task DeleteContact(int ContactId)
        {
            IsBusy = true;
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Contact Confirmation"].Value);
            if (ConfirmDelete)
            {
                using var context = contextFactory.CreateDbContext();
                var contactToDelete = await context.Contacts.FindAsync(ContactId);
                if (contactToDelete is not null)
                {
                    context.Remove(contactToDelete);
                    await context.SaveChangesAsync();
                    await RefreshComponent();
                }
            }
            IsBusy = false;
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
