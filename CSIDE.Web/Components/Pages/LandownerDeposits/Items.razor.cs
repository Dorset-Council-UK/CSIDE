using BlazorBootstrap;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Services;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.LandownerDeposits
{
    public partial class Items(ILandownerDepositService landownerDepositService, ILogger<Items> logger, IPlacesSearchService placesSearchService)
    {
        private List<BreadcrumbItem>? NavItems;

        [SupplyParameterFromQuery]
        private string[]? ParishIds { get; set; }
        [SupplyParameterFromQuery]
        private string? ParishId { get; set; }
        [SupplyParameterFromQuery]
        private string? Location { get; set; }

        private ICollection<LandownerDeposit>? SearchResults;

        private const int MaxResults = 1000;
        private bool IsBusy { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Landowner Deposit Title"], Href="landowner-deposits" },
                new BreadcrumbItem{ Text = localizer["Search Results Title"], IsCurrentPage = true },
            ];
            try
            {
                IsBusy = true;

                SearchResults = await landownerDepositService.GetLandownerDepositsBySearchParameters(
                    ParishIds,
                    ParishId,
                    Location,
                    MaxResults);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred rendering the landowner deposits list component");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
