using BlazorBootstrap;
using CSIDE.Components.DMMO;
using CSIDE.Components.Mapping;
using CSIDE.Data;
using CSIDE.Data.Models.DMMO;
using CSIDE.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace CSIDE.Components.Pages.DMMO
{
    public partial class Create(IDbContextFactory<ApplicationDbContext> contextFactory,
                                NavigationManager navigationManager,
                                ILogger<Create> logger,
                                IRightsOfWayHelperService rightsOfWayHelperService)
    {
        private Application? DMMOApplication { get; set; }
        private ApplicationClaimedStatus[]? ClaimedStatuses;
        private ApplicationCaseStatus[]? CaseStatuses;
        private ApplicationType[]? ApplicationTypes;


        private DMMOEditForm? childDMMOEditForm;

        private List<BreadcrumbItem>? NavItems;

        private CreateMap? createMap;
        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }
        private bool GeometryIsValid { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
            new BreadcrumbItem{ Text = localizer["DMMO Abbreviation"], Href="DMMO" },
                new BreadcrumbItem{ Text = localizer["Create New DMMO Label"], IsCurrentPage = true }
            ];
            IsBusy = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                ClaimedStatuses = await context.DMMOApplicationClaimedStatuses.AsNoTracking().OrderBy(s => s.Id).ToArrayAsync();
                CaseStatuses = await context.DMMOApplicationCaseStatuses.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                ApplicationTypes = await context.DMMOApplicationTypes.AsNoTracking().OrderBy(p => p.Id).ToArrayAsync();
                DMMOApplication = new()
                {
                    Geom = null,
                    ApplicationDetails = "",
                    ClaimedStatusId = ClaimedStatuses.Select(l => l.Id).FirstOrDefault(),
                    CaseStatusId = CaseStatuses.Select(o => o.Id).FirstOrDefault(),
                    ApplicationTypeId = ApplicationTypes.Select(r => r.Id).FirstOrDefault()
                };
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
                        using var context = contextFactory.CreateDbContext();
                        context.Add(DMMOApplication);
                        await context.SaveChangesAsync();
                        //redirect
                        navigationManager.NavigateTo($"DMMO/Details/{DMMOApplication.Id}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["DMMO Details Title", DMMOApplication.Id]];
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

            CSIDE.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer, rightsOfWayHelperService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Line String"));
            if (result.IsValid)
            {
                GeometryIsValid = true;
                if (DMMOApplication is not null)
                {
                    Geometry geom = featureCollection.First().Geometry;
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
            using var context = contextFactory.CreateDbContext();
            //create a polygon from the bounding box
            var bboxPolygon = new Polygon(new LinearRing([new(bbox[0], bbox[1]), new(bbox[2], bbox[1]), new(bbox[2], bbox[3]), new(bbox[0], bbox[3]), new(bbox[0], bbox[1])]))
            {
                SRID = 27700
            };
            var routes = await context.Routes.Where(g => g.Geom.Intersects(bboxPolygon)).Select(g => g.Geom).ToArrayAsync();
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
                SRID = 27700
            };
            var routes = await rightsOfWayHelperService.GetNearestRouteAsync(selectionPoint, 10);
            //convert route to geojson
            var geoJsonWriter = new GeoJsonWriter();
            var routesGeoJson = geoJsonWriter.Write(routes?.Geom);
            return routesGeoJson;
        }
    }
}
