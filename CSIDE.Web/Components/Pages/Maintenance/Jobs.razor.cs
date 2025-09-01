using BlazorBootstrap;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.Maintenance
{
    public partial class Jobs(IMaintenanceJobsService maintenanceJobsService, ILogger<Jobs> logger)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string? RouteId { get; set; }
        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? AssignedToTeamId { get; set; }
        [SupplyParameterFromQuery]
        private string? JobPriorityId { get; set; }
        [SupplyParameterFromQuery]
        private string? JobStatusId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? LogDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? LogDateTo { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? CompletedDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? CompletedDateTo { get; set; }
        [SupplyParameterFromQuery]
        private bool? IsComplete { get; set; }

        private IReadOnlyCollection<Job>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Maintenance Title"], Href="Maintenance" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
            try
            {
                IsBusy = true;

                SearchResults = await maintenanceJobsService.GetMaintenanceJobsBySearchParameters(
                    RouteId,
                    ParishIds,
                    ParishId,
                    AssignedToTeamId,
                    JobPriorityId,
                    IsComplete,
                    JobStatusId,
                    LogDateFrom,
                    LogDateTo,
                    CompletedDateFrom,
                    CompletedDateTo,
                    MaxResults
                    );

                
            }catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the jobs list component");
            }
            finally
            {
                IsBusy = false;
            }
        }


    }
}
