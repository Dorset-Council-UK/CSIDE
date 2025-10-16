using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace CSIDE.Web.Components.Pages.Infrastructure
{
    public partial class Index(
        IInfrastructureService infrastructureService,
        IMaintenanceJobsService maintenanceJobsService,
        ISharedDataService sharedDataService,
        NavigationManager navigationManager)
    {
        private List<BreadcrumbItem>? NavItems;
        private InfrastructureSearch? SearchParams;
        private string? InfrastructureIDSearch;
        private ICollection<InfrastructureType> InfrastructureTypes = [];
        private IReadOnlyCollection<Parish> Parishes { get; set; } = [];
        private IReadOnlyCollection<Team> MaintenanceTeams { get; set; } = [];

        private string? InfrastructureIDSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;

        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }


        protected override async Task OnInitializedAsync()
        {
            NavItems =
        [
            new() { Text = localizer["Home Title"], Href = "" },
            new() { Text = localizer["Infrastructure Title"], IsCurrentPage = true },
        ];
            Parishes = await sharedDataService.GetParishes();
            InfrastructureTypes = await infrastructureService.GetInfrastructureTypeOptions();
            MaintenanceTeams = await maintenanceJobsService.GetMaintenanceTeams();
            SearchParams = new();
        }
        private async Task OnInfrastructureIDSearchSubmit()
        {
            if (InfrastructureIDSearch is not null)
            {
                IsBusy = true;
                InfrastructureIDSearchErrorMessage = null;
                try
                {
                    if (!string.IsNullOrEmpty(IDPrefixOptions.Value.Infrastructure))
                    {
                        //remove any left in place prefixes
                        if (InfrastructureIDSearch.StartsWith(IDPrefixOptions.Value.Infrastructure, StringComparison.OrdinalIgnoreCase))
                        {
                            InfrastructureIDSearch = InfrastructureIDSearch[IDPrefixOptions.Value.Infrastructure.Length..].Trim();
                        }
                    }
                    if (int.TryParse(InfrastructureIDSearch, CultureInfo.InvariantCulture, out int InfrastructureIDSearchInt))
                    {
                        var infrastructureExists = await infrastructureService.GetInfrastructureItemById(InfrastructureIDSearchInt) is not null;
                        if (infrastructureExists)
                        {
                            navigationManager.NavigateTo($"Infrastructure/Details/{InfrastructureIDSearchInt}");
                            return;
                        }

                        InfrastructureIDSearchErrorMessage = localizer["Infrastructure Not Found Error Message", InfrastructureIDSearch];
                    }
                    else
                    {
                        InfrastructureIDSearchErrorMessage = localizer["Infrastructure Not Found Error Message", InfrastructureIDSearch];
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnInfrastructureSearchSubmit()
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
                        navigationManager.NavigateTo($"Infrastructure/Items?{qs}");
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
