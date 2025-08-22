using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Web.Components.Shared
{
    public partial class ContactsList(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        [Parameter]
        public Contact[]? Contacts { get; set; }
        [Parameter]
        public EventCallback<Contact> OnSubmit { get; set; }
        [Parameter]
        public EventCallback OnRefresh { get; set; }
        [Parameter]
        public bool IsEditable { get; set; }

        private Modal AddContactModal = default!;
        private bool IsBusy { get; set; }
        private Contact? NewContact { get; set; }
        private ContactType[]? ContactTypes { get; set; }
        private string? ErrorMessage { get; set; }

        private ContactEditForm? NewContactForm;

        protected override async Task OnParametersSetAsync()
        {
            NewContact = new();
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
            if (await NewContactForm!.ValidateAsync())
            {
                IsBusy = true;
                if (OnSubmit.HasDelegate)
                {
                    await OnSubmit.InvokeAsync(NewContact);
                }
                IsBusy = false;
                await HideAddContactModal();
                await RefreshComponent();
            }
        }

        private async Task ShowAddContactModal()
        {
            await AddContactModal.ShowAsync();
        }
        private async Task HideAddContactModal()
        {
            await AddContactModal.HideAsync();
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
