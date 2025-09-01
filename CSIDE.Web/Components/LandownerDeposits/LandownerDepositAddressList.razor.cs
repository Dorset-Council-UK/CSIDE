using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Data.Services;
using CSIDE.Data.Validators.LandownerDeposits;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.LandownerDeposits;

public partial class LandownerDepositAddressList(ILandownerDepositService landownerDepositService, IPlacesSearchService addressSearchService, IJSRuntime JS, ILogger<LandownerDepositAddressList> logger)
{
    [Parameter]
    public required ICollection<LandownerDepositAddress>? Addresses { get; set; }
    [Parameter, EditorRequired]
    public required int LandownerDepositId { get; init; }
    [Parameter, EditorRequired]
    public required int LandownerDepositSecondaryId { get; init; }
    [Parameter]
    public bool IsEditable { get; set; }
    public bool IsBusy { get; set; }
    private bool ManualAddressFormShown = false;
    private string? ErrorMessage { get; set; }
    private string? AddressSearchInput { get; set; }
    private List<SimpleAddress>? AddressSearchAddresses { get; set; }
    private FluentValidationValidator? LandownerDepositAddressValidator;
    private LandownerDepositAddress NewLandownerDepositAddress { get; set; } = default!;
    private Modal AddAddressModal = default!;

    private async Task OpenAddAddressModal()
    {
        ErrorMessage = null;
        NewLandownerDepositAddress = new() { LandownerDepositId = LandownerDepositId, LandownerDepositSecondaryId = LandownerDepositSecondaryId };
        await AddAddressModal.ShowAsync();
    }

    private async Task DeleteLandownerDepositAddress(int LandownerDepositId, long UPRN)
    {
        IsBusy = true;
        bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete Landowner Deposit Address Confirmation"].Value);
        if (ConfirmDelete)
        {
            await landownerDepositService.DeleteAddressFromLandownerDeposit(LandownerDepositId, LandownerDepositSecondaryId, UPRN);
            await RefreshComponent();
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
                if (AddressSearchAddresses.Count == 0)
                {
                    ErrorMessage = localizer["Address Search No Results Message"];
                }
            }
            else
            {
                //validation error
                ErrorMessage = localizer["Enter At Least X Characters Validation Message", 3];
            }
        }
        catch (Exception ex)
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
            var LandownerDepositAddressToAdd = new LandownerDepositAddress()
            {
                LandownerDepositId = LandownerDepositId,
                LandownerDepositSecondaryId = LandownerDepositSecondaryId,
                UPRN = address.UPRN,
                Address = address.Address,
            };
            //validate with fluent validation 
            var validator = new LandownerDepositAddressValidator(landownerDepositService, localizer);
            var validationResult = await validator.ValidateAsync(LandownerDepositAddressToAdd);

            if (!validationResult.IsValid)
            {
                ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return;
            }
            await landownerDepositService.AddAddressToLandownerDeposit(LandownerDepositAddressToAdd);
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
            if (await LandownerDepositAddressValidator!.ValidateAsync())
            {
                //submit
                await landownerDepositService.AddAddressToLandownerDeposit(NewLandownerDepositAddress);
                await AddAddressModal.HideAsync();
                await RefreshComponent();
                ManualAddressFormShown = false; //always hide it again once used to discourage use
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred linking an address to a landowner deposit");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshComponent()
    {
        Addresses = await landownerDepositService.GetLandownerDepositAddressesByDepositId(LandownerDepositId, LandownerDepositSecondaryId);
        ErrorMessage = null;
    }
}
