using BlazorBootstrap;
using CSIDE.Components.Infrastructure;
using CSIDE.Components.Mapping;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using NetTopologySuite.Features;
using FluentValidation;
using CSIDE.Services;

namespace CSIDE.Components.Pages.Infrastructure
{
    public partial class Edit(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Edit> logger, IRightsOfWayHelperService geometryValidationService)
    {
        [Parameter]
        public int InfrastructureId { get; set; }

        private InfrastructureItem? InfrastructureItem { get; set; }
        private InfrastructureType[]? InfrastructureTypes { get; set; }
       
        private InfrastructureItemEditForm? childInfrastructureItemEditForm;

        private List<BreadcrumbItem>? NavItems;
        private Modal routeValidationModal = default!;
        private Modal errorModal = default!;

        private EditMap? editMap;
        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
                new BreadcrumbItem{ Text = localizer["Infrastructure Title"], Href="Infrastructure" },
                new BreadcrumbItem{ Text = localizer["Infrastructure Edit Title", InfrastructureId], IsCurrentPage = true }
            ];
            IsBusy = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                InfrastructureTypes = await context.InfrastructureTypes.AsNoTracking().OrderBy(n => n.Name).ToArrayAsync();
                InfrastructureItem = await context.Infrastructure.IgnoreAutoIncludes().FirstOrDefaultAsync(i => i.Id == InfrastructureId);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SubmitFormAsync()
        {
            if (InfrastructureItem is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await childInfrastructureItemEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (InfrastructureItem is not null)
                    {
                        using var context = contextFactory.CreateDbContext();

                        //get the existing job to enable the smarter change tracker.
                        //Without this, all properties are identified as tracked, since
                        //the DbContext is different from when the entity was queried
                        var existingInfra = await context.Infrastructure.FindAsync(InfrastructureItem.Id) ?? throw new Exception($"Infrastructure Item being edited (ID: {InfrastructureItem.Id}) was not found prior to updating");

                        context.Entry(existingInfra).CurrentValues.SetValues(InfrastructureItem);

                        await context.SaveChangesAsync();
                        //redirect
                        NavigateBackToInfrastructureDetailsPage();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["Infrastructure Details Title", InfrastructureItem.Id]];
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "Error saving infrastructure item");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToInfrastructureDetailsPage()
        {
            navigationManager.NavigateTo($"Infrastructure/Details/{InfrastructureId}");
        }

        ////TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)

        private async Task ValidateGeometry(string features)
        {
            //check the geometry is a single point and is on a valid route
            GeoJsonReader _geoJsonReader = new();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer, geometryValidationService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Single Point", "Point On Route"));
            if (result.IsValid)
            {
                //get route code and update Job.Geom
                if (InfrastructureItem is not null)
                {
                    InfrastructureItem.Geom = featureCollection.First().Geometry.Centroid;
                    InfrastructureItem.Geom.SRID = 27700;

                    var NearestRoute = await geometryValidationService.GetNearestRouteAsync(InfrastructureItem.Geom);
                    if (NearestRoute is not null)
                    {
                        InfrastructureItem.RouteId = NearestRoute.RouteCode;
                    }
                }
            }
            else
            {
                // Check to see what the error is and show appropriate error message
                // first check if the geometry was invalid
                if (result.Errors.Any(failure => string.Equals(failure.ErrorCode, "GEOM_OUTSIDE_BOUNDS", StringComparison.OrdinalIgnoreCase)) || result.Errors.Exists(failure => string.Equals(failure.ErrorCode, "INVALID_GEOM", StringComparison.OrdinalIgnoreCase)))
                {
                    //show generic error
                    await ShowGeometryValidationErrorModal();
                }
                // if its valid, check to see if there was no route nearby
                // NOTE I know it seems backwards to test this way round, but if you don't,
                // invalid geometries always come back saying 'no route found', which is not the right message
                // TODO - Improve logic through use of conditional validation
                else if (result.Errors.Any(failure => string.Equals(failure.ErrorCode, "NO_ROUTE_NEARBY", StringComparison.OrdinalIgnoreCase)))
                {
                    //we assigned the geom at this point in case the user goes ahead with overriding the route ID
                    if (InfrastructureItem is not null)
                    {
                        InfrastructureItem.Geom = featureCollection.First().Geometry.Centroid;
                        InfrastructureItem.Geom.SRID = 27700;
                    }
                    await ShowRouteValidationModal();
                }
            }
            StateHasChanged();
        }

        private async Task ShowRouteValidationModal()
        {
            await routeValidationModal.ShowAsync();
        }

        private async Task ShowGeometryValidationErrorModal()
        {
            await errorModal.ShowAsync();
        }

        private async Task HideRouteValidationModal()
        {
            await routeValidationModal.HideAsync();
        }

        private async Task ClearDrawnGeometries()
        {
            if (InfrastructureItem is not null)
            {
                InfrastructureItem.Geom = null;
                InfrastructureItem.RouteId = null;
            }
            await editMap!.ClearDrawnGeometries();
            await routeValidationModal.HideAsync();
            await errorModal.HideAsync();
        }
    }
}
