using BlazorBootstrap;
using CSIDE.Components.DMMO;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Pages.DMMO.Orders
{
    public partial class Create(
        IDbContextFactory<ApplicationDbContext> contextFactory, 
        NavigationManager navigationManager,
        ILogger<Create> logger) {
        private List<BreadcrumbItem>? NavItems;
        [Parameter]
        public int DMMOApplicationId { get; init; }

        private Application? DMMOApplication { get; set; }
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
                new BreadcrumbItem{ Text = localizer["DMMO Create Order Title", DMMOApplicationId], IsCurrentPage = true }
            ];
            IsBusy = true;
            using var context = contextFactory.CreateDbContext();
            DMMOApplication = await context.DMMOApplication.FindAsync(DMMOApplicationId);
            if(DMMOApplication is not null)
            {
                DecisionOfSecStateOptions = await context.OrderDecisionsOfSecState
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToArrayAsync();
                DeterminationProcessOptions = await context.OrderDeterminationProcesses
                    .AsNoTracking()
                    .OrderBy(p => p.Name)
                    .ToArrayAsync();
                Order = new DMMOOrder() { ApplicationId = DMMOApplicationId };
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
                        context.Add(Order);
                        await context.SaveChangesAsync();
                        //redirect
                        navigationManager.NavigateTo($"DMMO/Details/{Order.ApplicationId}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["DMMO Order Details Title", Order.ApplicationId]];
                    logger.LogError(ex, "An concurrency conflict occurred when creating an Order for a DMMO");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating an Order for a DMMO");
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