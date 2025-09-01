using BlazorBootstrap;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.Infrastructure
{
    public partial class Items(IInfrastructureService infrastructureService, ILogger<Items> logger)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string? RouteId { get; set; }
        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? MaintenanceTeamId { get; set; }
        [SupplyParameterFromQuery]
        private string? LoggedById { get; set; }
        [SupplyParameterFromQuery]
        private string? InfrastructureTypeId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? InstallationDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? InstallationDateTo { get; set; }

        private ICollection<InfrastructureItem>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Infrastructure Title"], Href="Infrastructure" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
            try
            {
                IsBusy = true;
                SearchResults = await infrastructureService.GetInfrastructureItemBySearchParameters(
                    RouteId,
                    ParishIds,
                    ParishId,
                    MaintenanceTeamId,
                    InfrastructureTypeId,
                    InstallationDateFrom,
                    InstallationDateTo,
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
