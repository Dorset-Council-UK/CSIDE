using BlazorBootstrap;
using CSIDE.Web.Components.Mapping;
using CSIDE.Web.Components.RightsOfWay;
using CSIDE.Data.Models.RightsOfWay;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace CSIDE.Web.Components.Pages.RightsOfWay
{
    public partial class Edit(
        IRightsOfWayService rightsOfWayService,
        IMaintenanceJobsService maintenanceJobsService,
        NavigationManager navigationManager,
        ILogger<Edit> logger)
    {
        [Parameter]
        public required string RouteID { get; set; }

        private Data.Models.RightsOfWay.Route? Route { get; set; }
        private IReadOnlyCollection<LegalStatus> LegalStatuses { get; set; } = [];
        private IReadOnlyCollection<RouteType> RouteTypes { get; set; } = [];
        private IReadOnlyCollection<OperationalStatus> OperationalStatuses { get; set; } = [];
        private IReadOnlyCollection<Data.Models.Maintenance.Team> MaintenanceTeams { get; set; } = [];
        

        private RoWEditForm? childRouteEditForm;

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
                new BreadcrumbItem{ Text = localizer["Rights of Way Title"], Href="rights-of-way" },
                new BreadcrumbItem{ Text = localizer["Rights of Way Details Title", RouteID], Href=$"rights-of-way/Details/{RouteID}"},
                new BreadcrumbItem{ Text = localizer["Rights of Way Edit Title", RouteID], IsCurrentPage = true },
            ];
            IsBusy = true;
            try
            {
                LegalStatuses = await rightsOfWayService.GetLegalStatusOptions();
                RouteTypes = await rightsOfWayService.GetRouteTypeOptions();
                OperationalStatuses = await rightsOfWayService.GetOperationalStatusOptions();
                MaintenanceTeams = await maintenanceJobsService.GetMaintenanceTeams();

                Route = await rightsOfWayService.GetRouteByCode(RouteID);
                GeometryIsValid = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SubmitFormAsync()
        {
            if (Route is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (GeometryIsValid && await childRouteEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (Route is not null)
                    {
                        await rightsOfWayService.UpdateRoute(Route);
                        //redirect
                        NavigateBackToRouteDetailsPage();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["Rights of Way Details Title", Route.RouteCode]];
                    logger.LogError(ex, "An concurrency conflict occurred when updating a Right of Way");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred updating a Right of Way");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToRouteDetailsPage()
        {
            navigationManager.NavigateTo($"rights-of-way/Details/{RouteID}");
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
                if (Route is not null)
                {
                    Geometry geom = featureCollection[0].Geometry;
                    foreach(var feat in featureCollection.Skip(1))
                    {
                        geom = feat.Geometry.Union(geom);
                    }
                    if (geom.OgcGeometryType == OgcGeometryType.LineString)
                    {
                        var geometryFactory = new GeometryFactory();
                        geom = geometryFactory.CreateMultiLineString([(LineString)geom]);
                    }
                    Route.Geom = (MultiLineString)geom;
                    Route.Geom.SRID = 27700;
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
            if(bbox.Length != 4)
            {
                throw new ArgumentException("Bounding box must have 4 values", paramName: nameof(bbox));
            }
            //create a polygon from the bounding box
            var bboxPolygon = new Polygon(new LinearRing([new(bbox[0], bbox[1]), new(bbox[2], bbox[1]), new(bbox[2], bbox[3]), new(bbox[0], bbox[3]), new(bbox[0], bbox[1])]))
            {
                SRID = 27700,
            };
            ICollection<Geometry> routes = await rightsOfWayService.GetRoutesIntersecting(bboxPolygon);
            //exclude the current route being edited
            if (Route is not null)
            {
                routes = routes.Where(r => !r.EqualsExact(Route.Geom)).ToArray();
            }
            //convert routes to geojson
            var featureCollection = new FeatureCollection();
            foreach (var route in routes)
            {
                var attributes = new AttributesTable();
                var feature = new Feature(route, attributes);
                featureCollection.Add(feature);
            }

            var geoJsonWriter = new GeoJsonWriter();
            var routesGeoJson = geoJsonWriter.Write(featureCollection);
            return routesGeoJson;
        }
    }
}
