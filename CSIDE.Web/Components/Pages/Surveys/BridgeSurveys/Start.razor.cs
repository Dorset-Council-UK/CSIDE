using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Web.Components.Pages.Surveys.BridgeSurveys
{
    public partial class Start(IDbContextFactory<ApplicationDbContext> contextFactory,
                               NavigationManager navigationManager,
                               ILogger<Start> logger,
                               ToastService toastService)
    {
        
        [Parameter]
        public int InfraId { get; set; }

        private List<BreadcrumbItem>? NavItems;

        private InfrastructureItem? InfrastructureItem { get; set; }
        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }

        protected override void OnInitialized()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Surveys Title"], Href="surveys" },
                new BreadcrumbItem{ Text = localizer["Start Bridge Survey Title"], IsCurrentPage = true },
            ];
        }

        protected override async Task OnParametersSetAsync()
        {
            IsBusy = true;

            //fetch infrastructure
            using var context = contextFactory.CreateDbContext();

            InfrastructureItem = await context.Infrastructure.FindAsync(InfraId);

            //TODO - Make this more flexible
            if (InfrastructureItem is null || !string.Equals(InfrastructureItem.InfrastructureType?.Name, "Bridge", StringComparison.OrdinalIgnoreCase))
            {
                //not an infrastructure item or not a bridge
                toastService.Notify(new ToastMessage(ToastType.Danger, localizer["Survey Not A Bridge Error Message"]));
                navigationManager.NavigateTo("surveys/bridge/new");
                return;
            }
            IsBusy = false;
        }

        private async Task SubmitFormAsync()
        {
            if (InfrastructureItem is not null)
            {
                IsBusy = true;
                try
                {
                    using var context = contextFactory.CreateDbContext();
                    var newSurvey = new Data.Models.Surveys.BridgeSurvey
                    {
                        InfrastructureItemId = InfrastructureItem.Id,
                    };
                    context.BridgeSurveys.Add(newSurvey);
                    await context.SaveChangesAsync();
                    navigationManager.NavigateTo($"surveys/bridge/{newSurvey.Id}/confirm-location");
                }
                catch(Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "Error creating a new bridge survey");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}