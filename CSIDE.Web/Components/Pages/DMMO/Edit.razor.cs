using BlazorBootstrap;
using CSIDE.Data.Models.DMMO;
using CSIDE.Data.Services;
using CSIDE.Web.Components.DMMO;
using CSIDE.Web.Components.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace CSIDE.Web.Components.Pages.DMMO
{
    public partial class Edit(IDMMOService dmmoService, NavigationManager navigationManager, ILogger<Create> logger, IRightsOfWayService rightsOfWayService)
    {
        [Parameter]
        public int DMMOApplicationId { get; set; }
        private DMMOApplication? DMMOApplication { get; set; }
        private ICollection<ApplicationClaimedStatus> ClaimedStatuses = [];
        private ICollection<ApplicationCaseStatus> CaseStatuses = [];
        private ICollection<ApplicationType> ApplicationTypes = [];
        private ICollection<ApplicationDirectionOfSecState> DirectionsOfSecState = [];


        private DMMOEditForm? childDMMOEditForm;

        private List<BreadcrumbItem>? NavItems;

        private EditMap? editMap;
        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }
        private bool GeometryIsValid { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href="DMMO" },
                new BreadcrumbItem{ Text = localizer["DMMO Details Title", $"{IDPrefixOptions.Value.DMMO}{DMMOApplicationId}"], Href=$"DMMO/Details/{DMMOApplicationId}"},
                new BreadcrumbItem{ Text = localizer["Edit DMMO Label", $"{IDPrefixOptions.Value.DMMO}{DMMOApplicationId}"], IsCurrentPage = true },
            ];
            IsBusy = true;
            try
            {
                ClaimedStatuses = await dmmoService.GetClaimedStatusOptions();
                CaseStatuses = await dmmoService.GetCaseStatusOptions();
                ApplicationTypes = await dmmoService.GetApplicationTypeOptions();
                DirectionsOfSecState = await dmmoService.GetDirectionOfSecStateOptions();
                DMMOApplication = await dmmoService.GetDMMOApplicationById(DMMOApplicationId);
                GeometryIsValid = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SubmitFormAsync()
        {
            if (DMMOApplication is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (GeometryIsValid && await childDMMOEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (DMMOApplication is not null)
                    {
                        await dmmoService.UpdateDMMO(DMMOApplication);
                        //redirect
                        navigationManager.NavigateTo($"DMMO/Details/{DMMOApplication.Id}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["DMMO Details Title", $"{IDPrefixOptions.Value.DMMO}{DMMOApplication.Id}"]];
                    logger.LogError(ex, "An concurrency conflict occurred when creating a DMMO");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating a DMMO");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToRouteSearchPage()
        {
            navigationManager.NavigateTo($"DMMO");
        }

        //TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)

        private async Task ValidateGeometry(string features)
        {
            GeoJsonReader _geoJsonReader = new();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Data.Validators.Geometry.GeometryValidator validator = new(localizer, rightsOfWayService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Line String"));
            if (result.IsValid)
            {
                GeometryIsValid = true;
                if (DMMOApplication is not null)
                {
                    Geometry geom = featureCollection[0].Geometry;
                    foreach (var feat in featureCollection.Skip(1))
                    {
                        geom = feat.Geometry.Union(geom);
                    }
                    if (geom.OgcGeometryType == OgcGeometryType.LineString)
                    {
                        var geometryFactory = new GeometryFactory();
                        geom = geometryFactory.CreateMultiLineString([(LineString)geom]);
                    }
                    DMMOApplication.Geom = (MultiLineString)geom;
                    DMMOApplication.Geom.SRID = 27700;
                }
            }
            else
            {
                GeometryIsValid = false;
            }
            StateHasChanged();
        }

        private async Task<string> GetSnappables(double[] bbox)
        {
            if (bbox.Length != 4)
            {
                throw new ArgumentException("Bounding box must have 4 values", paramName: nameof(bbox));
            }
            //create a polygon from the bounding box
            var bboxPolygon = new Polygon(new LinearRing([new(bbox[0], bbox[1]), new(bbox[2], bbox[1]), new(bbox[2], bbox[3]), new(bbox[0], bbox[3]), new(bbox[0], bbox[1])]))
            {
                SRID = 27700,
            };
            var routes = await rightsOfWayService.GetRoutesIntersecting(bboxPolygon);
            //convert routes to geojson
            var featureCollection = new FeatureCollection();
            foreach (var route in routes)
            {
                var attributes = new AttributesTable(StringComparer.OrdinalIgnoreCase);
                var feature = new Feature(route, attributes);
                featureCollection.Add(feature);
            }

            var geoJsonWriter = new GeoJsonWriter();
            var routesGeoJson = geoJsonWriter.Write(featureCollection);
            return routesGeoJson;
        }

        private async Task<string> GetRoute(double[] coordinates)
        {
            if (coordinates.Length != 2)
            {
                throw new ArgumentException("Coordinates must be an array of 2 values", paramName: nameof(coordinates));
            }
            var selectionPoint = new Point(coordinates[0], coordinates[1])
            {
                SRID = 27700,
            };
            var routes = await rightsOfWayService.GetNearestRoute(selectionPoint, 10);
            //convert route to geojson
            var geoJsonWriter = new GeoJsonWriter();
            var routesGeoJson = geoJsonWriter.Write(routes?.Geom);
            return routesGeoJson;
        }
    }
}
