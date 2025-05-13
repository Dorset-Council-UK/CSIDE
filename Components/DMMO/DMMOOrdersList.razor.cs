using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CSIDE.Data.Models.DMMO;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data;
using BlazorBootstrap;

namespace CSIDE.Components.DMMO
{
    public partial class DMMOOrdersList(IJSRuntime JS, IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        [Parameter]
        public Order[]? Orders { get; set; }
        [Parameter]
        public int DMMOApplicationId { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = false;

        private bool IsBusy { get; set; } = false;

        private Modal OrderDetailsModal = default!;
        private Order? SelectedOrder { get; set; }

        private async Task DeleteOrder(int ApplicationId, int OrderId)
        {
            IsBusy = true;
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Order Confirmation"].Value);
            if (ConfirmDelete)
            {
                using var context = contextFactory.CreateDbContext();
                var orderToDelete = await context.DMMOOrders.FindAsync(OrderId, ApplicationId);
                if (orderToDelete is not null)
                {
                    context.Remove(orderToDelete);
                    await context.SaveChangesAsync();
                    await RefreshComponent();
                }
            }
            IsBusy = false;
        }

        private async Task ViewOrder(int OrderId)
        {
            SelectedOrder = Orders?.Where(o => o.OrderId == OrderId).FirstOrDefault();
            await OrderDetailsModal.ShowAsync();
        }

        private async Task RefreshComponent()
        {
            using var context = contextFactory.CreateDbContext();
            Orders = await context.DMMOOrders.Where(o => o.ApplicationId == DMMOApplicationId).ToArrayAsync();
            StateHasChanged();
        }
    }

}