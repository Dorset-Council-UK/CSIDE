using BlazorBootstrap;
using CSIDE.Components.Mapping;
using CSIDE.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using NetTopologySuite.Features;
using FluentValidation;
using CSIDE.Components.RightsOfWay;
using NetTopologySuite.Geometries;
using CSIDE.Data.Models.RightsOfWay;
using System.Globalization;
using CSIDE.Services;

namespace CSIDE.Components.Pages.RightsOfWay
{
    public partial class Create(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Create> logger, IRightsOfWayHelperService geometryValidationService)
    {
        private Data.Models.RightsOfWay.Route? Route { get; set; }
        private LegalStatus[]? LegalStatuses { get; set; }
        private RouteType[]? RouteTypes { get; set; }
        private OperationalStatus[]? OperationalStatuses { get; set; }
        private Data.Models.Maintenance.Team[]? MaintenanceTeams { get; set; }


        private RoWEditForm? childRouteEditForm;

        private List<BreadcrumbItem>? NavItems;

        private CreateMap? createMap;
        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }
        private string? ParishValidationMessage { get; set; }
        private bool GeometryIsValid { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            NavItems =
            [
                new BreadcrumbItem{ Text = localizer["Home Title"], Href = "" },
                new BreadcrumbItem{ Text = localizer["Rights of Way Title"], Href="rights-of-way" },
                new BreadcrumbItem{ Text = localizer["Create New Right of Way Label"], IsCurrentPage = true }
            ];
            IsBusy = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                LegalStatuses = await context.RouteLegalStatuses.AsNoTracking().OrderBy(s => s.Id).ToArrayAsync();
                RouteTypes = await context.RouteTypes.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                OperationalStatuses = await context.RouteOperationalStatuses.AsNoTracking().OrderBy(p => p.Id).ToArrayAsync();
                MaintenanceTeams = await context.MaintenanceTeams.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                Route = new()
                {
                    Geom = null,
                    LegalStatusId = LegalStatuses.Select(l => l.Id).FirstOrDefault(),
                    OperationalStatusId = OperationalStatuses.Where(o => !o.IsClosed).Select(o => o.Id).FirstOrDefault(),
                    RouteCode = "XXX/XX",
                    RouteTypeId = RouteTypes.Select(r => r.Id).FirstOrDefault()
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
                        using var context = contextFactory.CreateDbContext();
                        context.Add(Route);
                        await context.SaveChangesAsync();
                        //redirect
                        navigationManager.NavigateTo($"rights-of-way/Details/{Route.RouteCode}");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["Rights of Way Details Title", Route.RouteCode]];
                    logger.LogError(ex, "An concurrency conflict occurred when creating a Right of Way");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating a Right of Way");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private void NavigateBackToRouteSearchPage()
        {
            navigationManager.NavigateTo($"rights-of-way");
        }

        //TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)

        private async Task ValidateGeometry(string features)
        {
            GeoJsonReader _geoJsonReader = new();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer, geometryValidationService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Line String"));
            if (result.IsValid)
            {
                GeometryIsValid = true;
                if (Route is not null)
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
                    Route.Geom = (MultiLineString)geom;
                    Route.Geom.SRID = 27700;

                    //fetch route code
                    Route.RouteCode = await GetNextAvailableRouteCode(geom);
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

        private async Task<string> GetNextAvailableRouteCode(Geometry geom)
        {
            using var context = contextFactory.CreateDbContext();
            //get the parish that the geometry falls within
            var parish = await context.Parishes.Where(p => p.Geom.Intersects(geom)).ToListAsync();
            var code = "XXX";
            if (parish.Count == 0)
            {
                //no parish found. Warn user
                ParishValidationMessage = localizer["No Parish Found Error Message"];


            }
            else if (parish.Count == 1)
            {
                //single parish found
                //get parish code
                code = context.ParishCodes.Where(pc => pc.ParishId == parish.First().ParishId).Select(pc => pc.Code).FirstOrDefault();
                ParishValidationMessage = null;

            }
            else if (parish.Count > 1)
            {
                //multiple parishes found
                var bestParish = await context.Parishes.Where(p => p.Geom.Intersects(geom)).OrderByDescending(p => p.Geom.Intersection(geom).Length).FirstOrDefaultAsync();
                if (bestParish is not null)
                {
                    code = context.ParishCodes.Where(pc => pc.ParishId == bestParish.ParishId).Select(pc => pc.Code).FirstOrDefault();
                    ParishValidationMessage = localizer["Multiple Parish Codes Found Error Message", bestParish.Name];
                }
            }
            if (code is not null)
            {
                //get next available number
                var routes = context.Routes.Where(r => r.RouteCode.StartsWith($"{code}/")).Select(r => r.RouteCode).ToArray();
                //extract the highest number from the route code (e.g. 'W1/13' would return 13)
                if(routes is null || routes.Length == 0)
                {
                    return $"{code}/1";
                }
                var highestNumber = routes.Select(r => int.Parse(r.Replace(code, "").Replace("/", ""), CultureInfo.InvariantCulture)).Max();
                return $"{code}/{highestNumber + 1}";
            }
            ParishValidationMessage = localizer["No Parish Found Error Message"];
            return "XXX/XX";
        }
    }
}
