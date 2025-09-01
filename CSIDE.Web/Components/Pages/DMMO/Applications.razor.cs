using BlazorBootstrap;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.DMMO
{
    public partial class Applications(IDMMOService dmmoService, ILogger<Applications> logger)
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
        private string? ApplicationClaimedStatusId { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ApplicationDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ApplicationDateTo { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateFrom { get; set; }
        [SupplyParameterFromQuery]
        private DateOnly? ReceivedDateTo { get; set; }

        private ICollection<Application>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href = "DMMO" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];

            try
            {
                IsBusy = true;
                SearchResults = await dmmoService.GetDMMOApplicationsBySearchParameters(
                    ParishIds,
                    ParishId,
                    ApplicationTypeId,
                    ApplicationCaseStatusId,
                    ApplicationClaimedStatusId,
                    Location,
                    ApplicationDateFrom,
                    ApplicationDateTo,
                    ReceivedDateFrom,
                    ReceivedDateTo);

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
