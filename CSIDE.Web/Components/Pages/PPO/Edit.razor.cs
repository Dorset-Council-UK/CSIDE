using BlazorBootstrap;
using CSIDE.Web.Components.Mapping;
using CSIDE.Web.Components.PPO;
using CSIDE.Data.Models.PPO;
using CSIDE.Data.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace CSIDE.Web.Components.Pages.PPO
{
    public partial class Edit(IPPOService ppoService, NavigationManager navigationManager, ILogger<Edit> logger, IRightsOfWayService geometryValidationService)
    {
        [Parameter]
        public int PPOApplicationId { get; set; }
        private PPOApplication? PPOApplication { get; set; }
        private IReadOnlyCollection<ApplicationCaseStatus> CaseStatuses = [];
        private IReadOnlyCollection<PPOLegislation> ApplicationTypes = [];
        private IReadOnlyCollection<ApplicationIntent> Intents = [];
        private IReadOnlyCollection<ApplicationPriority> Priorities = [];
        private List<int> SelectedIntents { get; set; } = [];


        private PPOEditForm? childPPOEditForm;

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
                new BreadcrumbItem{ Text = localizer["PPO Abbreviation"], Href="PPO" },
                new BreadcrumbItem{ Text = localizer["PPO Details Title", $"{IDPrefixOptions.Value.PPO}{PPOApplicationId}"], Href=$"PPO/Details/{PPOApplicationId}"},
                new BreadcrumbItem{ Text = localizer["Edit PPO Label", $"{IDPrefixOptions.Value.PPO}{PPOApplicationId}"], IsCurrentPage = true },
            ];
            IsBusy = true;
            try
            {
                CaseStatuses = await ppoService.GetPPOCaseStatusOptions();
                ApplicationTypes = await ppoService.GetPPOApplicationTypeOptions();
                Intents = await ppoService.GetPPOApplicationIntents();
                Priorities = await ppoService.GetPPOApplicationPriorities();
                PPOApplication = await ppoService.GetPPOApplicationById(PPOApplicationId);
                if(PPOApplication is not null)
                {
                    SelectedIntents = PPOApplication.PPOIntents.Select(i => i.IntentId).ToList();
                }
                GeometryIsValid = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SubmitFormAsync()
        {
            if (PPOApplication is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (GeometryIsValid && await childPPOEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (PPOApplication is not null)
                    {
                        await ppoService.UpdatePPO(PPOApplication, SelectedIntents);
                        //redirect
                        navigationManager.NavigateTo($"PPO/Details/{PPOApplication.Id}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["PPO Details Title", PPOApplication.Id]];
                    logger.LogError(ex, "An concurrency conflict occurred when creating a PPO");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating a PPO");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToRouteSearchPage()
        {
            navigationManager.NavigateTo($"PPO");
        }

        //TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)

        private async Task ValidateGeometry(string features)
        {
            GeoJsonReader _geoJsonReader = new();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Data.Validators.Geometry.GeometryValidator validator = new(localizer, geometryValidationService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Line String"));
            if (result.IsValid)
            {
                GeometryIsValid = true;
                if (PPOApplication is not null)
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
                    PPOApplication.Geom = (MultiLineString)geom;
                    PPOApplication.Geom.SRID = 27700;
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
            var routes = await geometryValidationService.GetRoutesIntersecting(bboxPolygon);
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
            var routes = await geometryValidationService.GetNearestRoute(selectionPoint, 10);
            //convert route to geojson
            var geoJsonWriter = new GeoJsonWriter();
            var routesGeoJson = geoJsonWriter.Write(routes);
            return routesGeoJson;
        }
    }
}
