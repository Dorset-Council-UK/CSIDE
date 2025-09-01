using BlazorBootstrap;
using CSIDE.Web.Components.Infrastructure;
using CSIDE.Web.Components.Mapping;
using CSIDE.Data.Models.Infrastructure;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace CSIDE.Web.Components.Pages.Infrastructure
{
    public partial class Edit(IInfrastructureService infrastructureService, NavigationManager navigationManager, ILogger<Edit> logger, IRightsOfWayService rightsOfWayService)
    {
        [Parameter]
        public int InfrastructureId { get; set; }

        private InfrastructureItem? InfrastructureItem { get; set; }
        private ICollection<InfrastructureType> InfrastructureTypes { get; set; } = [];
       
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
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Infrastructure Title"], Href="Infrastructure" },
                new BreadcrumbItem{ Text = localizer["Infrastructure Details Title", $"{IDPrefixOptions.Value.Infrastructure}{InfrastructureId}"], Href=$"Infrastructure/Details/{InfrastructureId}"},
                new BreadcrumbItem{ Text = localizer["Infrastructure Edit Title", $"{IDPrefixOptions.Value.Infrastructure}{InfrastructureId}"], IsCurrentPage = true },
            ];
            IsBusy = true;
            try
            {
                InfrastructureTypes = await infrastructureService.GetInfrastructureTypeOptions();
                InfrastructureItem = await infrastructureService.GetInfrastructureItemById(InfrastructureId) ?? throw new Exception($"Infrastructure Item with ID {InfrastructureId} was not found");
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

                        await infrastructureService.UpdateInfrastructureItem(InfrastructureItem);
                        //redirect
                        NavigateBackToInfrastructureDetailsPage();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["Infrastructure Details Title", $"{IDPrefixOptions.Value.Infrastructure}{InfrastructureItem.Id}"]];
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

            CSIDE.Data.Validators.Geometry.GeometryValidator validator = new(localizer, rightsOfWayService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Single Point", "Point On Route"));
            if (result.IsValid)
            {
                //get route code and update Job.Geom
                if (InfrastructureItem is not null)
                {
                    InfrastructureItem.Geom = featureCollection[0].Geometry.Centroid;
                    InfrastructureItem.Geom.SRID = 27700;

                    var NearestRoute = await rightsOfWayService.GetNearestRoute(InfrastructureItem.Geom);
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
                if (result.Errors.Exists(failure => string.Equals(failure.ErrorCode, "GEOM_OUTSIDE_BOUNDS", StringComparison.OrdinalIgnoreCase)) || result.Errors.Exists(failure => string.Equals(failure.ErrorCode, "INVALID_GEOM", StringComparison.OrdinalIgnoreCase)))
                {
                    //show generic error
                    await ShowGeometryValidationErrorModal();
                }
                // if its valid, check to see if there was no route nearby
                // NOTE I know it seems backwards to test this way round, but if you don't,
                // invalid geometries always come back saying 'no route found', which is not the right message
                // TODO - Improve logic through use of conditional validation
                else if (result.Errors.Exists(failure => string.Equals(failure.ErrorCode, "NO_ROUTE_NEARBY", StringComparison.OrdinalIgnoreCase)))
                {
                    //we assigned the geom at this point in case the user goes ahead with overriding the route ID
                    if (InfrastructureItem is not null)
                    {
                        InfrastructureItem.Geom = featureCollection[0].Geometry.Centroid;
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
