using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorBootstrap;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;

namespace CSIDE.Web.Components.DMMO
{
    public partial class DMMOOrdersList(IJSRuntime JS, IDMMOService dmmoService)
    {
        [Parameter]
        public ICollection<DMMOOrder>? Orders { get; set; }
        [Parameter]
        public int DMMOApplicationId { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = false;

        private bool IsBusy { get; set; } = false;

        private Modal OrderDetailsModal = default!;
        private DMMOOrder? SelectedOrder { get; set; }

        private async Task DeleteOrder(int ApplicationId, int OrderId)
        {
            IsBusy = true;
            bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Order Confirmation"].Value);
            if (ConfirmDelete)
            {
                await dmmoService.DeleteDMMOOrder(ApplicationId, OrderId);
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
            Orders = await dmmoService.GetDMMOOrdersByApplicationId(DMMOApplicationId);
            StateHasChanged();
        }
    }

}