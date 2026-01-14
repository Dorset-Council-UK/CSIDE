using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.PPO
{
    public partial class Applications()
    {
        private List<BreadcrumbItem>? NavItems;


        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Location { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationCaseStatusId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationLegislationId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationTypeId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationPriorityId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateTo { get; set; }
        [SupplyParameterFromQuery]
        private bool? IsPublic { get; set; }

        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["PPO Abbreviation"], Href = "PPO" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
        }
    }
}
