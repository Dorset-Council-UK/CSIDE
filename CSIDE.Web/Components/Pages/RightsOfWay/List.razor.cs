using BlazorBootstrap;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.RightsOfWay
{
    public partial class List(IRightsOfWayService rightsOfWayService, ILogger<List> logger)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string? RouteId { get; set; }
        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Name { get; set; }
        [SupplyParameterFromQuery]
        private string? MaintenanceTeamId { get; set; }
        [SupplyParameterFromQuery]
        private string? OperationalStatusId { get; set; }
        [SupplyParameterFromQuery]
        private string? RouteTypeId { get; set; }

        private ICollection<Data.Models.RightsOfWay.Route>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Rights of Way Title"], Href="rights-of-way" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
            try {
                IsBusy = true;

                SearchResults = await rightsOfWayService.GetRoutesBySearchParameters(
                    RouteId,
                    Name,
                    ParishIds,
                    ParishId,
                    MaintenanceTeamId,
                    OperationalStatusId,
                    RouteTypeId,
                    MaxResults);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the jobs list component");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
