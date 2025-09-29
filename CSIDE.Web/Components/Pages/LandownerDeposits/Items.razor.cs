using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.LandownerDeposits
{
    public partial class Items()
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Location { get; set; }


        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Title"], Href="landowner-deposits" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
        }
    }
}
