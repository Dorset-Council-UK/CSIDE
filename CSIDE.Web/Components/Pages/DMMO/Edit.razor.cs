using BlazorBootstrap;
using CSIDE.Data;
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
using System.Globalization;

namespace CSIDE.Web.Components.Pages.DMMO
{
    public partial class Edit(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Create> logger, IRightsOfWayHelperService geometryValidationService)
    {
        [Parameter]
        public int DMMOApplicationId { get; set; }
        private Application? DMMOApplication { get; set; }
        private ApplicationClaimedStatus[]? ClaimedStatuses;
        private ApplicationCaseStatus[]? CaseStatuses;
        private ApplicationType[]? ApplicationTypes;
        private ApplicationDirectionOfSecState[]? DirectionsOfSecState;


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
                using var context = contextFactory.CreateDbContext();
                ClaimedStatuses = await context.DMMOApplicationClaimedStatuses.AsNoTracking().OrderBy(s => s.Id).ToArrayAsync();
                CaseStatuses = await context.DMMOApplicationCaseStatuses.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                ApplicationTypes = await context.DMMOApplicationTypes.AsNoTracking().OrderBy(p => p.Id).ToArrayAsync();
                DirectionsOfSecState = await context.DMMOApplicationDirectionsOfSecState.AsNoTracking().OrderBy(p => p.Id).ToArrayAsync();
                DMMOApplication = await context.DMMOApplication.IgnoreAutoIncludes().Include(d => d.DMMOParishes).FirstOrDefaultAsync(d => d.Id == DMMOApplicationId);
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

                        //get the existing job to enable the smarter change tracker.
                        //Without this, all properties are identified as tracked, since
                        //the DbContext is different from when the entity was queried
                        var existingApp = await context.DMMOApplication.FindAsync(DMMOApplication.Id) ?? throw new Exception(string.Create(CultureInfo.InvariantCulture, $"DMMO Application being edited (ID: {DMMOApplication.Id}) was not found prior to updating"));

                        // Store the original version for concurrency checking
                        uint originalVersion = DMMOApplication.Version;
                        // Update values
                        context.Entry(existingApp).CurrentValues.SetValues(DMMOApplication);
                        // Restore original version to ensure concurrency check works
                        context.Entry(existingApp).Property(j => j.Version).OriginalValue = originalVersion;

                        await context.SaveChangesAsync();
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

            CSIDE.Data.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer, geometryValidationService);

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
            using var context = contextFactory.CreateDbContext();
            //create a polygon from the bounding box
            var bboxPolygon = new Polygon(new LinearRing([new(bbox[0], bbox[1]), new(bbox[2], bbox[1]), new(bbox[2], bbox[3]), new(bbox[0], bbox[3]), new(bbox[0], bbox[1])]))
            {
                SRID = 27700,
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
            using var context = contextFactory.CreateDbContext();
            var selectionPoint = new Point(coordinates[0], coordinates[1])
            {
                SRID = 27700,
            };
            var routes = await context.Routes
                .Where(g => g.Geom.IsWithinDistance(selectionPoint, 10))
                .OrderBy(g => g.Geom.Distance(selectionPoint))
                .Select(g => g.Geom)
                .Take(1).SingleOrDefaultAsync();
            //convert route to geojson
            var geoJsonWriter = new GeoJsonWriter();
            var routesGeoJson = geoJsonWriter.Write(routes);
            return routesGeoJson;
        }
    }
}
