using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Pages.DMMO.Orders
{
    public partial class Details(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        private List<BreadcrumbItem>? NavItems;
        [Parameter]
        public int DMMOApplicationId { get; init; }
        [Parameter]
        public int OrderId { get; init; }

        private Order? Order { get; set; }
        private bool IsBusy { get; set; } = false;
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href="DMMO" },
                new BreadcrumbItem{ Text = localizer["DMMO Details Title", DMMOApplicationId], Href=$"DMMO/details/{DMMOApplicationId}" },
                new BreadcrumbItem{ Text = localizer["DMMO Order Details Title", DMMOApplicationId, OrderId], IsCurrentPage = true }
            ];
            IsBusy = true;
            using var context = contextFactory.CreateDbContext();
            Order = await context.DMMOOrders.FindAsync(OrderId,DMMOApplicationId);
            IsBusy = false;
        }

    }
}