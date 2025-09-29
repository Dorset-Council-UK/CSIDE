using BlazorBootstrap;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.DMMO
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
        private string? ApplicationTypeId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationClaimedStatusId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ApplicationDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ApplicationDateTo { get; set; }
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
                new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href = "DMMO" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];

        }
    }
}
