using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.RightsOfWay
{
    public partial class Index(
        IRightsOfWayService rightsOfWayService,
        IMaintenanceJobsService maintenanceJobsService,
        ISharedDataService sharedDataService,
        NavigationManager navigationManager)
    {

        private List<BreadcrumbItem>? NavItems;
        private Search? SearchParams;
        private string? RouteIDSearch;
        private IReadOnlyCollection<OperationalStatus> OperationalStatuses { get; set; } = [];
        private IReadOnlyCollection<RouteType> RouteTypes { get; set; } = [];
        private IReadOnlyCollection<Data.Models.Maintenance.Team> MaintenanceTeams { get; set; } = [];
        private IReadOnlyCollection<Parish> Parishes { get; set; } = [];
        private string? RouteIDSearchErrorMessage { get; set; }
        private FluentValidationValidator? _fluentValidationValidator;

        private bool UseMultiParishSelect { get; set; }
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavItems =
            [
                new() { Text = localizer["Home Title"], Href = "" },
                new() { Text = localizer["Rights of Way Title"], IsCurrentPage = true },
            ];

            OperationalStatuses = await rightsOfWayService.GetOperationalStatusOptions();
            RouteTypes = await rightsOfWayService.GetRouteTypeOptions();
            MaintenanceTeams = await maintenanceJobsService.GetMaintenanceTeams();
            Parishes = await sharedDataService.GetParishes();
            SearchParams = new();
        }

        private async Task OnRouteIDSearchSubmit()
        {
            if (RouteIDSearch is not null)
            {
                IsBusy = true;
                RouteIDSearchErrorMessage = null;
                try
                {
                    var routeExists = await rightsOfWayService.RouteExists(RouteIDSearch);
                    if (routeExists)
                    {
                        navigationManager.NavigateTo($"rights-of-way/Details/{RouteIDSearch}");
                        return;
                    }

                    RouteIDSearchErrorMessage = localizer["Right of Way Not Found Error Message", RouteIDSearch];

                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task OnRouteSearchSubmit()
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
                        navigationManager.NavigateTo($"rights-of-way/List?{qs}");
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
