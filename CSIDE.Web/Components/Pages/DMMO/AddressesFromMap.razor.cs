using BlazorBootstrap;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Data.Validators.DMMO;
using Microsoft.AspNetCore.Components;

namespace CSIDE.Web.Components.Pages.DMMO;

public partial class AddressesFromMap(IDMMOService dmmoService,
                                      ILogger<AddressesFromMap> logger,
                                      NavigationManager navigationManager)
{
    private List<BreadcrumbItem>? NavItems;

    [Parameter]
    public int Id { get; set; }

    private DMMOApplication? DMMOApplication { get; set; }
    private IReadOnlyCollection<SimpleAddress>? ExistingAddresses { get; set; }

    private string? ErrorMessage { get; set; }
    private bool IsBusy { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        NavItems =
        [
            new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
            new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href="DMMO" },
            new BreadcrumbItem{ Text = localizer["DMMO Details Title", $"{IDPrefixOptions.Value.DMMO}{Id}"], Href=$"DMMO/Details/{Id}" },
            new BreadcrumbItem{ Text = localizer["Add Addresses From Map Label"], IsCurrentPage = true },
        ];
        IsBusy = true;
        try
        {
            DMMOApplication = await dmmoService.GetDMMOApplicationById(Id);
            if (DMMOApplication is null)
            {
                throw new InvalidOperationException("DMMO Application not found");
            }
            ExistingAddresses = [.. DMMOApplication.DMMOAddresses.Select(x => new SimpleAddress(x.UPRN, x.Address))];
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["General Error Message"];
            logger.LogError(ex, "An error occurred getting the DMMO application for the address map");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task HandleSingleAddressSelected(SimpleAddress address)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            //submit
            var DMMOAddressToAdd = new DMMOAddress() { DMMOApplicationId = Id, UPRN = address.UPRN, Address = address.Address };
            //validate with fluent validation 
            var validator = new DMMOAddressValidator(localizer, dmmoService);
            var validationResult = await validator.ValidateAsync(DMMOAddressToAdd);

            if (!validationResult.IsValid)
            {
                ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return;
            }
            await dmmoService.AddDMMOAddress(DMMOAddressToAdd);
            await RefreshComponent();
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred adding an address to a DMMO");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task HandleMultipleAddressesSelected((List<SimpleAddress> addresses, bool finished) args)
    {
        var (addresses, finished) = args;
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var FullAddresses = await dmmoService.GetDMMOAddressesByApplicationId(Id);
            ExistingAddresses = FullAddresses.Select(x => new SimpleAddress(x.UPRN, x.Address)).ToList();
            foreach (var address in addresses.Where(a => ExistingAddresses is not null && !ExistingAddresses.Any(e => e.UPRN == a.UPRN)))
            {
                var DMMOAddressToAdd = new DMMOAddress() { DMMOApplicationId = Id, UPRN = address.UPRN, Address = address.Address };
                await dmmoService.AddDMMOAddress(DMMOAddressToAdd);
            }
            if (finished)
            {
                navigationManager.NavigateTo($"DMMO/Details/{Id}");
            }
            else
            {
                await RefreshComponent();
            }
            
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred adding multiple addresses to a DMMO");
        }
        finally
        {
            IsBusy = false;
        }

    }

    private async Task RefreshComponent()
    {
        IsBusy = true;
        try
        {
            DMMOApplication = await dmmoService.GetDMMOApplicationById(Id);
            if (DMMOApplication is null)
            {
                throw new InvalidOperationException("DMMO Application not found");
            }
            ExistingAddresses = [.. DMMOApplication.DMMOAddresses.Select(x => new SimpleAddress(x.UPRN, x.Address))];
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["General Error Message"];
            logger.LogError(ex, "An error occurred refreshing the DMMO application for the address map");
        }
        finally
        {
            IsBusy = false;
        }
    }

}