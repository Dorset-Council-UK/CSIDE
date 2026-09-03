using Blazored.FluentValidation;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;


namespace CSIDE.Web.Components.Shared
{
    public partial class ContactEditForm(ISharedDataService sharedDataService)
    {
        [Parameter]
        public Contact? ContactToEdit { get; set; }
        [Parameter]
        public IReadOnlyCollection<ContactType> ContactTypes { get; set; } = [];
        [Parameter]
        public bool IsBusy { get; set; }
        [Parameter]
        public EventCallback OnSubmit { get; set; }
        [Parameter]
        public EventCallback OnCancel { get; set; }

        private FluentValidationValidator? contactValidator;

        private string? ErrorMessage { get; set; }
        public IList<string> OrganisationSuggestions = [];

        protected override async Task OnInitializedAsync()
        {
            OrganisationSuggestions = await GetOrganisationSuggestions();
        }

        public async Task<bool> ValidateAsync()
        {
            return await contactValidator!.ValidateAsync();
        }

        private async Task HandleSubmit()
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync();
            }
        }
        private async Task HandleCancel()
        {
            if (OnCancel.HasDelegate)
            {
                await OnCancel.InvokeAsync();
            }
        }

        private async Task<IList<string>> GetOrganisationSuggestions()
        {
            var organisations = await sharedDataService.GetOrganisations();
            if (organisations is not null)
            {
                return [.. organisations.Select(o => o.OrganisationName ?? string.Empty).Order(StringComparer.OrdinalIgnoreCase)];
            }
            return [];
        }
    }
}