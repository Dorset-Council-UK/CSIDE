using BlazorBootstrap;
using CSIDE.Web.Components.DMMO;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Services;

namespace CSIDE.Web.Components.Pages.DMMO.Orders
{
    public partial class Edit(
        IDMMOService dmmoService, 
        NavigationManager navigationManager,
        ILogger<Edit> logger) {
        private List<BreadcrumbItem>? NavItems;
        [Parameter]
        public int DMMOApplicationId { get; init; }
        [Parameter]
        public int OrderId { get; init; }

        private DMMOOrder? Order { get; set; }
        private ICollection<OrderDecisionOfSecState>? DecisionOfSecStateOptions;
        private ICollection<OrderDeterminationProcess>? DeterminationProcessOptions;
        private OrderEditForm? childDMMOOrderEditForm;
        private bool IsBusy { get; set; } = false;
        private string? ErrorMessage { get; set; } = null;  
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href="DMMO" },
                new BreadcrumbItem{ Text = localizer["DMMO Details Title", DMMOApplicationId], Href=$"DMMO/details/{DMMOApplicationId}" },
                new BreadcrumbItem{ Text = localizer["Edit Order Title", DMMOApplicationId, OrderId], IsCurrentPage = true },
            ];
            IsBusy = true;
            Order = await dmmoService.GetDMMOOrderById(OrderId, DMMOApplicationId);
            if (Order is not null)
            {
                DecisionOfSecStateOptions = await dmmoService.GetOrderDecisionOfSecStateOptions();
                DeterminationProcessOptions = await dmmoService.GetOrderDeterminationProcessOptions();
            }
            IsBusy = false;
        }

        private async Task SubmitFormAsync()
        {
            if (Order is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await childDMMOOrderEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (Order is not null)
                    {
                        await dmmoService.UpdateDMMOOrder(OrderId, Order);
                        //redirect
                        navigationManager.NavigateTo($"DMMO/Details/{Order.DMMOApplicationId}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["DMMO Order Details Title", Order.DMMOApplicationId]];
                    logger.LogError(ex, "An concurrency conflict occurred when editing an Order for a DMMO");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred editing an Order for a DMMO");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToDMMOPage()
        {
            navigationManager.NavigateTo($"DMMO/Details/{DMMOApplicationId}");
        }

    }
}