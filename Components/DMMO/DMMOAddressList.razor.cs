using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using CSIDE.Services;
using CSIDE.Validators.DMMO;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Components.DMMO;

public partial class DMMOAddressList(IDbContextFactory<ApplicationDbContext> contextFactory, IPlacesSearchService addressSearchService, IJSRuntime JS, ILogger<DMMOAddressList> logger)
{
    [Parameter]
    public required ICollection<DMMOAddress>? Addresses { get; set; }
    [Parameter]
    public required int ApplicationId { get; init; }
    [Parameter]
    public bool IsEditable { get; set; }
    public bool IsBusy { get; set; }
    private bool ManualAddressFormShown = false;
    private string? ErrorMessage { get; set; }
    private string? AddressSearchInput { get; set; }
    private List<SimpleAddress>? AddressSearchAddresses { get; set; }
    private FluentValidationValidator? DMMOAddressValidator;
    private DMMOAddress NewDMMOAddress { get; set; } = default!;
    private Modal AddAddressModal = default!;

    private async Task OpenAddAddressModal()
    {
        ErrorMessage = null;
        NewDMMOAddress = new() { ApplicationId = ApplicationId };
        await AddAddressModal.ShowAsync();
    }

    private async Task DeleteDMMOAddress(int ApplicationId, long UPRN)
    {
        IsBusy = true;
        bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete DMMO Address Confirmation"].Value);
        if (ConfirmDelete)
        {
            using var context = contextFactory.CreateDbContext();
            var DMMOAddressToDelete = await context.DMMOAddresses.FindAsync([ApplicationId, UPRN]);
            if (DMMOAddressToDelete is not null)
            {
                context.Remove(DMMOAddressToDelete);
                await context.SaveChangesAsync();
                await RefreshComponent();
            }
        }
        IsBusy = false;
    }

    private async Task DoAddressSearch()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            //check AddressSearchInput
            if (AddressSearchInput?.Length >= 3)
            {
                //send to API and get results (use AddressSearchService)
                AddressSearchAddresses = await addressSearchService.GetAddresses(AddressSearchInput);
                if(AddressSearchAddresses.Count == 0)
                {
                    ErrorMessage = localizer["Address Search No Results Message"];
                }
            }
            else
            {
                //validation error
                ErrorMessage = localizer["Enter At Least X Characters Validation Message", 3];
            }
        }catch(Exception ex)
        {
            ErrorMessage = localizer["Address Search Error Message"];
            logger.LogError(ex, "Error searching for addresses");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddSingleAddress(SimpleAddress address)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            //submit
            var DMMOAddressToAdd = new DMMOAddress() { ApplicationId = ApplicationId, UPRN = address.UPRN, Address = address.Address };
            //validate with fluent validation 
            var validator = new DMMOAddressValidator(contextFactory, localizer);
            var validationResult = await validator.ValidateAsync(DMMOAddressToAdd);

            if (!validationResult.IsValid)
            {
                ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return;
            }
            using var context = contextFactory.CreateDbContext();
            context.Add(DMMOAddressToAdd);
            await context.SaveChangesAsync();
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

    private async Task SubmitFormAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            if (await DMMOAddressValidator!.ValidateAsync())
            {
                //submit
                using var context = contextFactory.CreateDbContext();
                context.Add(NewDMMOAddress);
                await context.SaveChangesAsync();
                await AddAddressModal.HideAsync();
                await RefreshComponent();
                ManualAddressFormShown = false; //always hide it again once used to discourage use
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred linking an address to a DMMO");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshComponent()
    {
        using var context = contextFactory.CreateDbContext();
        Addresses = await context.DMMOAddresses.Where(a => a.ApplicationId == ApplicationId).ToListAsync();
        ErrorMessage = null;
    }
}
