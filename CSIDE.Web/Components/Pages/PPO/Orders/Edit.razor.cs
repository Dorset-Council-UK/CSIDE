using BlazorBootstrap;
using CSIDE.Web.Components.PPO;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Services;

namespace CSIDE.Web.Components.Pages.PPO.Orders
{
    public partial class Edit(
        IPPOService ppoService, 
        NavigationManager navigationManager,
        ILogger<Edit> logger) {
        private List<BreadcrumbItem>? NavItems;
        [Parameter]
        public int PPOApplicationId { get; init; }
        [Parameter]
        public int OrderId { get; init; }

        private PPOOrder? Order { get; set; }
        private IReadOnlyCollection<OrderDecisionOfSecState> DecisionOfSecStateOptions = [];
        private IReadOnlyCollection<OrderDeterminationProcess> DeterminationProcessOptions = [];
        private OrderEditForm? childPPOOrderEditForm;
        private bool IsBusy { get; set; } = false;
        private string? ErrorMessage { get; set; } = null;  
        
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["PPO Abbreviation"], Href="PPO" },
                new BreadcrumbItem{ Text = localizer["PPO Details Title", $"{IDPrefixOptions.Value.PPO}{PPOApplicationId}"], Href=$"PPO/details/{PPOApplicationId}" },
                new BreadcrumbItem{ Text = localizer["Edit Order Title", $"{IDPrefixOptions.Value.PPO}{PPOApplicationId}", OrderId], IsCurrentPage = true },
            ];
            IsBusy = true;
            Order = await ppoService.GetPPOOrderById(OrderId, PPOApplicationId);
            if (Order is not null)
            {
                DecisionOfSecStateOptions = await ppoService.GetOrderDecisionOfSecStateOptions();
                DeterminationProcessOptions = await ppoService.GetOrderDeterminationProcessOptions();
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
            if (await childPPOOrderEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (Order is not null)
                    {
                        await ppoService.UpdatePPOOrder(OrderId, Order);
                        //redirect
                        navigationManager.NavigateTo($"PPO/Details/{Order.PPOApplicationId}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["PPO Order Details Title", $"{IDPrefixOptions.Value.PPO}{Order.PPOApplicationId}"]];
                    logger.LogError(ex, "An concurrency conflict occurred when editing an Order for a PPO");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred editing an Order for a PPO");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToPPOPage()
        {
            navigationManager.NavigateTo($"PPO/Details/{PPOApplicationId}");
        }

    }
}