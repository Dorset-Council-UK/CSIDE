using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorBootstrap;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Services;

namespace CSIDE.Web.Components.PPO
{
    public partial class PPOOrdersList(IJSRuntime JS, IPPOService ppoService)
    {
        [Parameter]
        public ICollection<PPOOrder>? Orders { get; set; }
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
                await ppoService.DeletePPOOrder(ApplicationId, OrderId);
                await RefreshComponent();
            }
            IsBusy = false;
        }

        private async Task ViewOrder(int OrderId)
        {
            SelectedOrder = Orders?.FirstOrDefault(o => o.OrderId == OrderId);
            await OrderDetailsModal.ShowAsync();
        }

        private async Task RefreshComponent()
        {
            Orders = await ppoService.GetPPOOrderByApplicationId(PPOApplicationId);
            StateHasChanged();
        }
    }

}