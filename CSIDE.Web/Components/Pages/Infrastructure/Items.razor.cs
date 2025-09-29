using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.Infrastructure
{
    public partial class Items()
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

        private bool IsBusy { get; set; }
        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Infrastructure Title"], Href="Infrastructure" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
        }
    }
}
