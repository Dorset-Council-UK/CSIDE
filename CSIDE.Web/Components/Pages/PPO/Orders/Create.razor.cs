using BlazorBootstrap;
using CSIDE.Web.Components.PPO;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using CSIDE.Data.Services;

namespace CSIDE.Web.Components.Pages.PPO.Orders
{
    public partial class Create(
        IPPOService ppoService,
        NavigationManager navigationManager,
        ILogger<Create> logger) {
        private List<BreadcrumbItem>? NavItems;
        [Parameter]
        public int PPOApplicationId { get; init; }

        private Application? PPOApplication { get; set; }
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
                new BreadcrumbItem{ Text = localizer["PPO Create Order Title", $"{IDPrefixOptions.Value.PPO}{PPOApplicationId}"], IsCurrentPage = true },
            ];
            IsBusy = true;
            PPOApplication = await ppoService.GetPPOApplicationById(PPOApplicationId);
            if (PPOApplication is not null)
            {
                DecisionOfSecStateOptions = await ppoService.GetOrderDecisionOfSecStateOptions();
                DeterminationProcessOptions = await ppoService.GetOrderDeterminationProcessOptions();
                Order = new PPOOrder() { ApplicationId = PPOApplicationId };
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
                        await ppoService.AddOrderToPPO(Order);
                        //redirect
                        navigationManager.NavigateTo($"PPO/Details/{Order.ApplicationId}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["PPO Order Details Title", $"{IDPrefixOptions.Value.PPO}{Order.ApplicationId}"]];
                    logger.LogError(ex, "An concurrency conflict occurred when creating an Order for a PPO");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating an Order for a PPO");
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