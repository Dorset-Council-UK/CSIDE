using BlazorBootstrap;
using CSIDE.Components.Mapping;
using CSIDE.Components.Infrastructure;
using CSIDE.Data;
using CSIDE.Data.Models.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using FluentValidation;

namespace CSIDE.Components.Pages.Infrastructure
{
    public partial class Create(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Create> logger)
    {
        private List<BreadcrumbItem>? NavItems;
        private Modal routeValidationModal = default!;
        private Modal errorModal = default!;

        private CreateMap? createMap;

        private InfrastructureItem? InfrastructureItem { get; set; }
        private InfrastructureType[]? InfrastructureTypes { get; set; }

        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }

        private InfrastructureItemEditForm? childInfrastructureItemEditForm;

        protected override async Task OnInitializedAsync()
        {
            NavItems =
            [
                new() { Text = localizer["Home Title"], Href = "/" },
                new() { Text = localizer["Infrastructure Title"], Href = "/Infrastructure" },
                new() { Text = localizer["Infrastructure Create Title"], IsCurrentPage = true }
            ];

            using var context = contextFactory.CreateDbContext();
            InfrastructureTypes = await context.InfrastructureTypes.OrderBy(n => n.Name).ToArrayAsync();
            InfrastructureItem = new()
            {
                InfrastructureTypeId = InfrastructureTypes.FirstOrDefault()?.Id,
            };

        }

        private async Task SubmitFormAsync()
        {
            if (IsBusy)
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

                        context.Infrastructure.Add(InfrastructureItem);
                        await context.SaveChangesAsync();
                       
                        //redirect
                        navigationManager.NavigateTo($"Infrastructure/Details/{InfrastructureItem.Id}");
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "Error creating infrastructure item");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        //TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)

        private async Task ValidateGeometry(string features)
        {
            //check the geometry is a single point and is on a valid route
            GeoJsonReader _geoJsonReader = new();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Single Point", "Point On Route"));
            if (result.IsValid)
            {
                //get route code and update InfrastructureItem.Geom
                if (InfrastructureItem is not null)
                {
                    InfrastructureItem.Geom = featureCollection.First().Geometry.Centroid;
                    InfrastructureItem.Geom.SRID = 27700;
                    using var context = contextFactory.CreateDbContext();
                    //TODO - Move this to shared location
                    var Route = await context.Routes.Where(r => r.Geom.Distance(InfrastructureItem.Geom) < 20).OrderBy(r => r.Geom.Distance(InfrastructureItem.Geom)).FirstOrDefaultAsync();
                    if (Route is not null)
                    {
                        InfrastructureItem.RouteId = Route.RouteCode;
                    }
                }
            }
            else
            {
                // Check to see what the error is and show appropriate error message
                // first check if the geometry was invalid
                if (result.Errors.Any(failure => failure.ErrorCode == "GEOM_OUTSIDE_BOUNDS") || result.Errors.Any(failure => failure.ErrorCode == "INVALID_GEOM"))
                {
                    //show generic error
                    await ShowGeometryValidationErrorModal();
                }
                // if its valid, check to see if there was no route nearby
                // NOTE I know it seems backwards to test this way round, but if you don't, 
                // invalid geometries always come back saying 'no route found', which is not the right message
                // TODO - Improve logic through use of conditional validation
                else if (result.Errors.Any(failure => failure.ErrorCode == "NO_ROUTE_NEARBY"))
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
            await createMap!.ClearDrawnGeometries();
            await routeValidationModal.HideAsync();
            await errorModal.HideAsync();
        }
    }
}
