using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CSIDE.Components.Maintenance
{
    public partial class ContactsList(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<ContactsList> logger)
    {
        [Parameter]
        public Job? Job { get; set; }
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

                try
                {
                    if (NewContact is not null && Job is not null)
                    {
                        using var context = contextFactory.CreateDbContext();

                        context.Contacts.Add(NewContact);
                        await context.SaveChangesAsync();
                        context.MaintenanceJobContact.Add(new JobContact { ContactId = NewContact.Id, JobId = Job.Id }); //UHRMMMM?
                        await context.SaveChangesAsync();
                        await HideAddContactModal();
                        //refresh component
                        await RefreshComponent();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating a maintenance contact");
                }
                finally
                {
                    IsBusy = false;
                }
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
            if (Job is not null)
            {
                NewContact = new();
                using var context = contextFactory.CreateDbContext();
                Job = await context.MaintenanceJobs.FindAsync([Job.Id]);
                ErrorMessage = null;
                StateHasChanged();
            }
        }
    }
}
