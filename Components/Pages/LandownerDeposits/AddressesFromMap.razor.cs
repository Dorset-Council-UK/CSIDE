using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.LandownerDeposits;
using CSIDE.Data.Models.Shared;
using CSIDE.Validators.LandownerDeposits;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Pages.LandownerDeposits;

public partial class AddressesFromMap(IDbContextFactory<ApplicationDbContext> contextFactory,
                                      ILogger<AddressesFromMap> logger,
                                      NavigationManager navigationManager)
{
    private List<BreadcrumbItem>? NavItems;

    [Parameter]
    public int Id { get; set; }

    private LandownerDeposit? LandownerDeposit { get; set; }
    private List<SimpleAddress>? ExistingAddresses { get; set; }

    private string? ErrorMessage { get; set; }
    private bool IsBusy { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        NavItems =
        [
            new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
            new BreadcrumbItem{ Text = localizer["Landowner Deposit Details Title", $"{IDPrefixOptions.Value.LandownerDeposit}{Id}"], Href=$"landowner-deposits/Details/{Id}" },
            new BreadcrumbItem{ Text = localizer["Add Addresses From Map Label"], IsCurrentPage = true }
        ];
        IsBusy = true;
        try
        {
            using var context = contextFactory.CreateDbContext();
            LandownerDeposit = await context.LandownerDeposits.FindAsync(Id);
            if (LandownerDeposit is null)
            {
                throw new InvalidOperationException("DMMO Application not found");
            }
            ExistingAddresses = LandownerDeposit.LandownerDepositAddresses
                .Select(x => new SimpleAddress(x.UPRN, x.Address))
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["General Error Message"];
            logger.LogError(ex, "An error occurred getting the landowner deposit for the address map");
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
            var LandownerDepositAddressToAdd = new LandownerDepositAddress() 
            { 
                LandownerDepositId = Id, 
                UPRN = address.UPRN, 
                Address = address.Address 
            };
            //validate with fluent validation 
            var validator = new LandownerDepositAddressValidator(contextFactory, localizer);
            var validationResult = await validator.ValidateAsync(LandownerDepositAddressToAdd);

            if (!validationResult.IsValid)
            {
                ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return;
            }
            using var context = contextFactory.CreateDbContext();
            context.Add(LandownerDepositAddressToAdd);
            await context.SaveChangesAsync();
            await RefreshComponent();
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred adding an address to a landowner deposit");
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
            using var context = contextFactory.CreateDbContext();

            //fetch existing addresses again just in case some have been added/removed between them being shown and being committed here
            //for single addresses we just use FluentValidation, but for multiple addresses its probably quicker to just check again
            ExistingAddresses = context.LandownerDepositAddresses.Where(d => d.LandownerDepositId == Id).Select(x => new SimpleAddress(x.UPRN, x.Address)).ToList();
            foreach (var address in addresses.Where(a => ExistingAddresses is not null && !ExistingAddresses.Any(e => e.UPRN == a.UPRN)))
            {
                var landownerDepositAddressToAdd = new LandownerDepositAddress() 
                { 
                    LandownerDepositId = Id, 
                    UPRN = address.UPRN, 
                    Address = address.Address 
                };
                context.Add(landownerDepositAddressToAdd);
            }
            await context.SaveChangesAsync();
            if (finished)
            {
                navigationManager.NavigateTo($"landowner-deposits/Details/{Id}");
            }
            else
            {
                await RefreshComponent();
            }
            
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred adding multiple addresses to a landowner deposit");
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
            using var context = contextFactory.CreateDbContext();
            LandownerDeposit = await context.LandownerDeposits.FindAsync(Id);
            if (LandownerDeposit is null)
            {
                throw new InvalidOperationException("Landowner deposit not found");
            }
            ExistingAddresses = LandownerDeposit.LandownerDepositAddresses
                .Select(x => new SimpleAddress(x.UPRN, x.Address))
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["General Error Message"];
            logger.LogError(ex, "An error occurred refreshing the landowner deposit for the address map");
        }
        finally
        {
            IsBusy = false;
        }
    }
}