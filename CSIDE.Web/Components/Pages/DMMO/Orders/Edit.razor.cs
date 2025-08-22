using BlazorBootstrap;
using CSIDE.Web.Components.DMMO;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Web.Components.Pages.DMMO.Orders
{
    public partial class Edit(
        IDbContextFactory<ApplicationDbContext> contextFactory, 
        NavigationManager navigationManager,
        ILogger<Edit> logger) {
        private List<BreadcrumbItem>? NavItems;
        [Parameter]
        public int DMMOApplicationId { get; init; }
        [Parameter]
        public int OrderId { get; init; }

        private DMMOOrder? Order { get; set; }
        private OrderDecisionOfSecState[]? DecisionOfSecStateOptions;
        private OrderDeterminationProcess[]? DeterminationProcessOptions;
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
            using var context = contextFactory.CreateDbContext();
            Order = await context.DMMOOrders
                .AsNoTracking()
                .Include(p => p.DecisionOfSecState)
                .FirstOrDefaultAsync(p => p.OrderId == OrderId && p.ApplicationId == DMMOApplicationId);
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
            if (await childDMMOOrderEditForm!.ValidateAsync())
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
                        var existingOrder = await context.DMMOOrders.FindAsync(Order.OrderId, Order.ApplicationId) ?? throw new Exception($"DMMO Order being edited (ID: {Order.ApplicationId}/{Order.OrderId}) was not found prior to updating");

                        context.Entry(existingOrder).CurrentValues.SetValues(Order);
                        await context.SaveChangesAsync();
                        //redirect
                        navigationManager.NavigateTo($"DMMO/Details/{Order.ApplicationId}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["DMMO Order Details Title", Order.ApplicationId]];
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