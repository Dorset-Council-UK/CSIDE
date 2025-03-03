using BlazorBootstrap;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Models.Shared;
using CSIDE.Services;
using CSIDE.Validators.DMMO;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace CSIDE.Components.Pages.DMMO;

public partial class AddressesFromMap(IDbContextFactory<ApplicationDbContext> contextFactory,
                                      ILogger<AddressesFromMap> logger,
                                      NavigationManager navigationManager)
{
    private List<BreadcrumbItem>? NavItems;

    [Parameter]
    public int Id { get; set; }

    private Application? DMMOApplication { get; set; }
    private List<SimpleAddress>? ExistingAddresses { get; set; }

    private string? ErrorMessage { get; set; }
    private bool IsBusy { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        NavItems =
        [
            new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
            new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href="DMMO" },
            new BreadcrumbItem{ Text = localizer["DMMO Details Title", Id], Href=$"DMMO/Details/{Id}" },
            new BreadcrumbItem{ Text = localizer["Add Addresses From Map Label"], IsCurrentPage = true }
        ];
        IsBusy = true;
        try
        {
            using var context = contextFactory.CreateDbContext();
            DMMOApplication = await context.DMMOApplication.FindAsync(Id);
            if (DMMOApplication is null)
            {
                throw new InvalidOperationException("DMMO Application not found");
            }
            ExistingAddresses = DMMOApplication.DMMOAddresses.Select(x => new SimpleAddress(x.UPRN, x.Address)).ToList();
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
            var DMMOAddressToAdd = new DMMOAddress() { ApplicationId = Id, UPRN = address.UPRN, Address = address.Address };
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
            ExistingAddresses = context.DMMOAddresses.Where(d => d.ApplicationId == Id).Select(x => new SimpleAddress(x.UPRN, x.Address)).ToList();
            foreach (var address in addresses.Where(a => ExistingAddresses is not null && !ExistingAddresses.Any(e => e.UPRN == a.UPRN)))
            {
                var DMMOAddressToAdd = new DMMOAddress() { ApplicationId = Id, UPRN = address.UPRN, Address = address.Address };
                context.Add(DMMOAddressToAdd);
            }
            await context.SaveChangesAsync();
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
            using var context = contextFactory.CreateDbContext();
            DMMOApplication = await context.DMMOApplication.FindAsync(Id);
            if (DMMOApplication is null)
            {
                throw new InvalidOperationException("DMMO Application not found");
            }
            ExistingAddresses = DMMOApplication.DMMOAddresses.Select(x => new SimpleAddress(x.UPRN, x.Address)).ToList();
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