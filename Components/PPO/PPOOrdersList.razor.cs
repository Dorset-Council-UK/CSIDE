using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data;
using BlazorBootstrap;
using CSIDE.Data.Models.PPO;

namespace CSIDE.Components.PPO
{
    public partial class PPOOrdersList(IJSRuntime JS, IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        [Parameter]
        public PPOOrder[]? Orders { get; set; }
        [Parameter]
        public int PPOApplicationId { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = false;

        private bool IsBusy { get; set; } = false;

        private Modal OrderDetailsModal = default!;
        private PPOOrder? SelectedOrder { get; set; }

        private async Task DeleteOrder(int ApplicationId, int OrderId)
        {
            IsBusy = true;
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Order Confirmation"].Value);
            if (ConfirmDelete)
            {
                using var context = contextFactory.CreateDbContext();
                var orderToDelete = await context.PPOOrders.FindAsync(OrderId, ApplicationId);
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
            Orders = await context.PPOOrders.Where(o => o.ApplicationId == PPOApplicationId).ToArrayAsync();
            StateHasChanged();
        }
    }

}