using CSIDE.Data;
using CSIDE.Data.Models.Shared;
using CSIDE.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CSIDE.Components.Shared
{
    public partial class MapBasedAddressSelector(IPlacesSearchService addressSearchService, ILogger<MapBasedAddressSelector> logger)
    {
        [Parameter]
        public required Geometry Geom { get; set; }
        [Parameter]
        public List<SimpleAddress>? ExistingAddresses { get; set; }
        [Parameter]
        public EventCallback<SimpleAddress> OnSingleAddressSelected { get; set; }
        [Parameter]
        public EventCallback<(List<SimpleAddress> addresses, bool finished)> OnMultipleAddressesSelected { get; set; }

        private List<SimpleAddress>? AddressSearchAddresses { get; set; }

        private bool IsFetchingAddresses { get; set; }
        private bool IsBusy { get; set; }

        private async Task GetAddresses(string features)
        {
            IsFetchingAddresses = true;
            try
            {
                AddressSearchAddresses = await addressSearchService.GetAddressesByGeometry(features);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred getting addresses from the map for a DMMO");
            }
            finally
            {
                IsFetchingAddresses = false;
            }
        }

        private async Task SingleAddressClicked(SimpleAddress address)
        {
            IsBusy = true;
            try
            {
                if (OnSingleAddressSelected.HasDelegate)
                {
                    await OnSingleAddressSelected.InvokeAsync(address);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred adding a single address to a DMMO from the map");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddAllAddressesClicked(bool finished)
        {
            IsBusy = true;
            try
            {
                if (OnMultipleAddressesSelected.HasDelegate && AddressSearchAddresses is not null)
                {
                    await OnMultipleAddressesSelected.InvokeAsync((AddressSearchAddresses, finished));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred adding multiple addresses to a DMMO from the map");
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}