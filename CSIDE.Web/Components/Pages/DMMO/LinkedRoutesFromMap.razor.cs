using BlazorBootstrap;
using CSIDE.Web.Components.DMMO;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using CSIDE.Data.Validators.DMMO;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CSIDE.Web.Components.Pages.DMMO;

public partial class LinkedRoutesFromMap(IDbContextFactory<ApplicationDbContext> contextFactory,
                                         IRightsOfWayHelperService rightsOfWayHelperService,
                                         ILogger<LinkedRoutesFromMap> logger)
{
    private List<BreadcrumbItem>? NavItems;

    [Parameter]
    public int Id { get; set; }

    private DMMOLinkedRoutesList? linkedRoutesComponent;

    private Application? DMMOApplication { get; set; }
    private DMMOLinkedRoute[]? ExistingLinkedRoutes { get; set; }

    private string? ErrorMessage { get; set; }
    private string? SelectionErrorMessage { get; set; }
    private bool IsBusy { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        NavItems =
        [
            new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
            new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href="DMMO" },
            new BreadcrumbItem{ Text = localizer["DMMO Details Title", Id], Href=$"DMMO/Details/{Id}" },
            new BreadcrumbItem{ Text = localizer["Add Linked Routes From Map Label"], IsCurrentPage = true },
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
            ExistingLinkedRoutes = [.. DMMOApplication.DMMOLinkedRoutes];
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["General Error Message"];
            logger.LogError(ex, "An error occurred getting the DMMO application for the linked routes map");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<Data.Models.RightsOfWay.Route[]> AddOrRemoveRouteAtCoordinate(double[] coordinates)
    {
        SelectionErrorMessage = null;
        IsBusy = true;
        StateHasChanged();
        try
        {
            if (coordinates.Length != 2)
            {
                throw new ArgumentException("Coordinates must be an array of 2 values", paramName: nameof(coordinates));
            }
            var selectionPoint = new Point(coordinates[0], coordinates[1])
            {
                SRID = 27700,
            };
            var route = await rightsOfWayHelperService.GetNearestRouteAsync(selectionPoint, 10);

            if (route is not null)
            {
                if (ExistingLinkedRoutes is not null && ExistingLinkedRoutes.Any(r => string.Equals(r.RouteId, route.RouteCode, StringComparison.OrdinalIgnoreCase)))
                {
                    //already exists, delete the route
                    await DeleteLinkedRoute(Id, route.RouteCode);
                }
                else
                {
                    //doesn't exist, add it
                    await AddSingleRoute(route.RouteCode);
                }
            }
            else
            {
                SelectionErrorMessage = localizer["No Routes Found At Click Error Message"];
                
            }
            return ExistingLinkedRoutes is not null ? [.. ExistingLinkedRoutes.Select(r => r.Route)] : [];
        }
        finally
        {
            IsBusy = false;
            StateHasChanged();
        }
        

    }

    private async Task AddSingleRoute(string RouteId)
    {
        ErrorMessage = null;
        try
        {
            //submit
            var DMMOLinkedRouteToAdd = new DMMOLinkedRoute() { ApplicationId = Id, RouteId = RouteId };
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
    }

    private async Task DeleteLinkedRoute(int ApplicationId, string RouteId)
    {
        ErrorMessage = null;
        try
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
        catch(Exception ex)
        {
            ErrorMessage = localizer["Save Error Message"];
            logger.LogError(ex, "An error occurred deleting a linked route from a DMMO");
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
            ExistingLinkedRoutes = [.. DMMOApplication.DMMOLinkedRoutes];
            if (linkedRoutesComponent != null)
            {
                await linkedRoutesComponent.RefreshComponent();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = localizer["General Error Message"];
            logger.LogError(ex, "An error occurred refreshing the DMMO application for the linked routes map");
        }
        finally
        {
            IsBusy = false;
        }
    }

}