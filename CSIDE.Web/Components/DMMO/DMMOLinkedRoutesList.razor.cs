using BlazorBootstrap;
using Blazored.FluentValidation;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using CSIDE.Data.Validators.DMMO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CSIDE.Web.Components.DMMO;

public partial class DMMOLinkedRoutesList(IDMMOService dmmoService,
                                          IRightsOfWayService rightsOfWayHelperService,
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
    private ICollection<Data.Models.RightsOfWay.Route> NearbyRoutes { get; set; } = [];

    private DMMOLinkedRoute NewDMMOLinkedRoute { get; set; } = default!;
    private Modal AddLinkedRouteModal = default!;

    private async Task OpenAddLinkedRoutesModal()
    {
        ErrorMessage = null;
        NewDMMOLinkedRoute = new() { DMMOApplicationId = ApplicationId, RouteId="" };
        await GetNearbyRoutes();
        await AddLinkedRouteModal.ShowAsync();
    }

    private async Task GetNearbyRoutes()
    {
        //get job location
        var application = await dmmoService.GetDMMOApplicationById(ApplicationId);

        if (application?.Geom is not null)
        {
            //get nearest 10 routes that intersect or are close to application
            var existingRoutes = LinkedRoutes?.Select(l => l.RouteId);
            NearbyRoutes = await rightsOfWayHelperService.GetNearestRoutes(application.Geom, 50, 10);
        }
    }

    private async Task AddSingleRoute(string RouteId)
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            //submit
            var DMMOLinkedRouteToAdd = new DMMOLinkedRoute() { DMMOApplicationId = ApplicationId, RouteId = RouteId };
            // validate with fluent validation 
            var validator = new DMMOLinkedRouteValidator(dmmoService, localizer, rightsOfWayHelperService);
            var validationResult = await validator.ValidateAsync(DMMOLinkedRouteToAdd);

            if (!validationResult.IsValid)
            {
                ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return;
            }
            await dmmoService.AddLinkedRouteToDMMO(DMMOLinkedRouteToAdd);
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
            await dmmoService.DeleteDMMOLinkedRoute(ApplicationId, RouteId);
            await RefreshComponent();
            
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
                await dmmoService.AddLinkedRouteToDMMO(NewDMMOLinkedRoute);
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
        LinkedRoutes = await dmmoService.GetDMMOLinkedRoutesByApplicationId(ApplicationId);
        ErrorMessage = null;
        StateHasChanged();
    }
}
