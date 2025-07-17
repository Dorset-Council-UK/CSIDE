using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Services;
using CSIDE.Validators.DMMO;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CSIDE.Components.DMMO;

public partial class DMMOLinkedRoutesList(IDbContextFactory<ApplicationDbContext> contextFactory,
                                          IRightsOfWayHelperService rightsOfWayHelperService,
                                          IJSRuntime JS,
                                          ILogger<DMMOLinkedRoutesList> logger)
{
    [Parameter]
    public required ICollection<DMMOLinkedRoute>? LinkedRoutes { get; set; }
    [Parameter]
    public required int ApplicationId { get; init; }
    [Parameter]
    public bool IsEditable { get; set; }
    public bool IsBusy { get; set; }

    private FluentValidationValidator? DMMOLinkedRouteValidator;
    private string? ErrorMessage { get; set; }
    private List<Data.Models.RightsOfWay.Route> NearbyRoutes { get; set; } = [];

    private DMMOLinkedRoute NewDMMOLinkedRoute { get; set; } = default!;
    private Modal AddLinkedRouteModal = default!;

    private async Task OpenAddLinkedRoutesModal()
    {
        ErrorMessage = null;
        NewDMMOLinkedRoute = new() { ApplicationId = ApplicationId, RouteId="" };
        await GetNearbyRoutes();
        await AddLinkedRouteModal.ShowAsync();
    }

    private async Task GetNearbyRoutes()
    {
        //get job location
        using var context = contextFactory.CreateDbContext();
        var applicationGeom = await context.DMMOApplication.IgnoreAutoIncludes().Where(d => d.Id == ApplicationId).Select(d => d.Geom).FirstOrDefaultAsync();
        if (applicationGeom is not null)
        {
            //get nearest 10 routes that intersect or are close to application
            var existingRoutes = LinkedRoutes?.Select(l => l.RouteId);
            NearbyRoutes = await context.Routes
                .Where(i => i.Geom.IsWithinDistance(applicationGeom, 50))
                .OrderBy(i => i.Geom.Distance(applicationGeom))
                .ToListAsync();
        }
    }

    private async Task AddSingleRoute(string RouteId)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            //submit
            var DMMOLinkedRouteToAdd = new DMMOLinkedRoute() { ApplicationId = ApplicationId, RouteId = RouteId };
            // validate with fluent validation 
            var validator = new DMMOLinkedRouteValidator(contextFactory, localizer, rightsOfWayHelperService);
            var validationResult = await validator.ValidateAsync(DMMOLinkedRouteToAdd);

            if (!validationResult.IsValid)
            {
                ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return;
            }
            using var context = contextFactory.CreateDbContext();
            context.Add(DMMOLinkedRouteToAdd);
            await context.SaveChangesAsync();
            await RefreshComponent();
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred adding a linked route to a DMMO");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteLinkedRoute(int ApplicationId, string RouteId)
    {
        IsBusy = true;
        bool ConfirmDelete = await JS.InvokeAsync<bool>("confirm", localizer["Delete DMMO Linked Route Confirmation"].Value);
        if (ConfirmDelete)
        {
            using var context = contextFactory.CreateDbContext();
            var DMMOLinkedRouteToDelete = await context.DMMOLinkedRoutes.FindAsync([ApplicationId, RouteId]);
            if (DMMOLinkedRouteToDelete is not null)
            {
                context.Remove(DMMOLinkedRouteToDelete);
                await context.SaveChangesAsync();
                await RefreshComponent();
            }
        }
        IsBusy = false;
    }

    private async Task SubmitFormAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            if (await DMMOLinkedRouteValidator!.ValidateAsync())
            {
                //submit
                using var context = contextFactory.CreateDbContext();
                context.Add(NewDMMOLinkedRoute);
                await context.SaveChangesAsync();
                await AddLinkedRouteModal.HideAsync();
                await RefreshComponent();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred linking a route to a DMMO");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task RefreshComponent()
    {
        using var context = contextFactory.CreateDbContext();
        LinkedRoutes = await context.DMMOLinkedRoutes.Where(a => a.ApplicationId == ApplicationId).ToListAsync();
        ErrorMessage = null;
        StateHasChanged();
    }
}
