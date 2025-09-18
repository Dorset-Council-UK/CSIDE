using BlazorBootstrap;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.PPO
{
    public partial class Applications(IPPOService ppoService, ILogger<Applications> logger)
    {
        private List<BreadcrumbItem>? NavItems;


        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Location { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationCaseStatusId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationTypeId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationIntentId { get; set; }
        [SupplyParameterFromQuery]
        private string? ApplicationPriorityId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateTo { get; set; }

        private IReadOnlyCollection<PPOApplication>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["PPO Abbreviation"], Href = "PPO" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];

            try
            {
                IsBusy = true;
                SearchResults = await ppoService.GetPPOApplicationsBySearchParameters(
                    ParishIds,
                    ParishId,
                    ApplicationTypeId,
                    ApplicationCaseStatusId,
                    ApplicationIntentId,
                    ApplicationPriorityId,
                    Location,
                    ReceivedDateFrom,
                    ReceivedDateTo,
                    MaxResults);

            }catch(Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the applications list component");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
