using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.Maintenance
{
    public partial class Jobs()
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string? RouteId { get; set; }
        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? AssignedToTeamId { get; set; }
        [SupplyParameterFromQuery]
        private string? JobPriorityId { get; set; }
        [SupplyParameterFromQuery]
        private string? JobStatusId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? LogDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? LogDateTo { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? CompletedDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? CompletedDateTo { get; set; }
        [SupplyParameterFromQuery]
        private bool? IsComplete { get; set; }

        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Maintenance Title"], Href="Maintenance" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
        }


    }
}
