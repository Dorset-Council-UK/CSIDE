using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Web.Components.Pages.RightsOfWay
{
    public partial class Index(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager)
    {

        private List<BreadcrumbItem>? NavItems;
        private Search? SearchParams;
        private string? RouteIDSearch;
        private OperationalStatus[]? OperationalStatuses { get; set; }
        private RouteType[]? RouteTypes { get; set; }
        private Data.Models.Maintenance.Team[]? MaintenanceTeams { get; set; }
        private Parish[]? Parishes { get; set; }
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

            using var context = contextFactory.CreateDbContext();
            OperationalStatuses = await context.RouteOperationalStatuses.ToArrayAsync();
            RouteTypes = await context.RouteTypes.ToArrayAsync();
            MaintenanceTeams = await context.MaintenanceTeams.ToArrayAsync();
            Parishes = await context.Parishes.OrderBy(p => p.Name).ToArrayAsync();
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
                    using var context = contextFactory.CreateDbContext();
                    var routeExists = await context.Routes.AnyAsync(j => j.RouteCode == RouteIDSearch);
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
