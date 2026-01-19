using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.DMMO
{
    public partial class Index(IDMMOService dmmoService, ISharedDataService sharedDataService, NavigationManager navigationManager)
    {
        private List<BreadcrumbItem>? NavItems;
        private DMMOSearch? SearchParams;
        private string? DMMOIDSearch;
        private ICollection<ApplicationClaimedStatus> ClaimedStatuses = [];
        private ICollection<ApplicationCaseStatus> CaseStatuses = [];
        private ICollection<ApplicationType> ApplicationTypes = [];
        private IReadOnlyCollection<Parish>? Parishes { get; set; }

        private string? DMMOSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;
        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavItems =
           [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], IsCurrentPage = true },
            ];

            Parishes = await sharedDataService.GetParishes();
            ClaimedStatuses = await dmmoService.GetClaimedStatuses();
            CaseStatuses = await dmmoService.GetCaseStatusOptions();
            ApplicationTypes = await dmmoService.GetApplicationTypes();

            SearchParams = new();
        }

        private async Task OnDMMOIDSearchSubmit()
        {
            if (DMMOIDSearch is not null)
            {
                IsBusy = true;
                DMMOSearchErrorMessage = null;
                try
                {
                    if (!string.IsNullOrEmpty(IDPrefixOptions.Value.DMMO))
                    {
                        //remove any left in place prefixes
                        if (DMMOIDSearch.StartsWith(IDPrefixOptions.Value.DMMO, StringComparison.OrdinalIgnoreCase))
                        {
                            DMMOIDSearch = DMMOIDSearch[IDPrefixOptions.Value.DMMO.Length..].Trim();
                        }
                    }
                    if (int.TryParse(DMMOIDSearch, CultureInfo.InvariantCulture, out int DMMOIDSearchInt))
                    {
                        var dmmoExists = await dmmoService.GetDMMOApplicationById(DMMOIDSearchInt) is not null;
                        if (dmmoExists)
                        {
                            navigationManager.NavigateTo($"DMMO/Details/{DMMOIDSearchInt}");
                            return;
                        }

                        DMMOSearchErrorMessage = localizer["DMMO Not Found Error Message", DMMOIDSearch];
                    }
                    else
                    {
                        DMMOSearchErrorMessage = localizer["DMMO Not Found Error Message", DMMOIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnDMMOSearchSubmit()
        {
            if (SearchParams is not null)
            {
                IsBusy = true;
                if (UseMultiParishSelect)
                {
                    SearchParams.ParishId = null;
                }
                else
                {
                    SearchParams.ParishIds = [];
                }
                try
                {
                    if (await _fluentValidationValidator!.ValidateAsync())
                    {
                        var qs = Helpers.QueryStringHelper.GetQueryString(SearchParams);
                        navigationManager.NavigateTo($"DMMO/Applications?{qs}");
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

    }
}
