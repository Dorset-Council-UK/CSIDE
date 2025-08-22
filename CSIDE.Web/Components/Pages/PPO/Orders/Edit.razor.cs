using BlazorBootstrap;
using CSIDE.Web.Components.PPO;
using CSIDE.Data;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Web.Components.Pages.PPO.Orders
{
    public partial class Edit(
        IDbContextFactory<ApplicationDbContext> contextFactory, 
        NavigationManager navigationManager,
        ILogger<Edit> logger) {
        private List<BreadcrumbItem>? NavItems;
        [Parameter]
        public int PPOApplicationId { get; init; }
        [Parameter]
        public int OrderId { get; init; }

        private PPOOrder? Order { get; set; }
        private OrderDecisionOfSecState[]? DecisionOfSecStateOptions;
        private OrderDeterminationProcess[]? DeterminationProcessOptions;
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
            using var context = contextFactory.CreateDbContext();
            Order = await context.PPOOrders
                .AsNoTracking()
                .Include(p => p.DecisionOfSecState)
                .FirstOrDefaultAsync(p => p.OrderId == OrderId && p.ApplicationId == PPOApplicationId);
            if (Order is not null)
            {
                DecisionOfSecStateOptions = await context.OrderDecisionsOfSecState
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToArrayAsync();
                DeterminationProcessOptions = await context.OrderDeterminationProcesses
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToArrayAsync();
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
                        using var context = contextFactory.CreateDbContext();
                        //get the existing order to enable the smarter change tracker.
                        //Without this, all properties are identified as tracked, since
                        //the DbContext is different from when the entity was queried
                        var existingOrder = await context.PPOOrders.FindAsync(Order.OrderId, Order.ApplicationId) ?? throw new Exception($"PPO Order being edited (ID: {Order.ApplicationId}/{Order.OrderId}) was not found prior to updating");

                        context.Entry(existingOrder).CurrentValues.SetValues(Order);
                        await context.SaveChangesAsync();
                        //redirect
                        navigationManager.NavigateTo($"PPO/Details/{Order.ApplicationId}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["PPO Order Details Title", $"{IDPrefixOptions.Value.PPO}{Order.ApplicationId}"]];
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