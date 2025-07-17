using BlazorBootstrap;
using CSIDE.Components.Mapping;
using CSIDE.Components.PPO;
using CSIDE.Data;
using CSIDE.Data.Models.PPO;
using CSIDE.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace CSIDE.Components.Pages.PPO
{
    public partial class Edit(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Edit> logger, IRightsOfWayHelperService geometryValidationService)
    {
        [Parameter]
        public int PPOApplicationId { get; set; }
        private Application? PPOApplication { get; set; }
        private ApplicationCaseStatus[]? CaseStatuses;
        private ApplicationType[]? ApplicationTypes;
        private ApplicationIntent[]? Intents;
        private ApplicationPriority[]? Priorities;
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
                new BreadcrumbItem{ Text = localizer["Edit PPO Label", $"{IDPrefixOptions.Value.PPO}{PPOApplicationId}"], IsCurrentPage = true }
            ];
            IsBusy = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                CaseStatuses = await context.PPOApplicationCaseStatuses.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                ApplicationTypes = await context.PPOApplicationTypes.AsNoTracking().OrderBy(p => p.Id).ToArrayAsync();
                Intents = await context.PPOApplicationIntents.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                Priorities = await context.PPOApplicationPriorities.AsNoTracking().OrderBy(p => p.SortOrder).ToArrayAsync();
                PPOApplication =
                    await context.PPOApplication
                    .IgnoreAutoIncludes()
                    .Include(p => p.PPOParishes)
                    .Include(p => p.PPOIntents)
                    .FirstOrDefaultAsync(p => p.Id == PPOApplicationId);
                SelectedIntents = PPOApplication!.PPOIntents.Select(i => i.IntentId).ToList();

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
                        using var context = contextFactory.CreateDbContext();

                        //get the existing job to enable the smarter change tracker
                        var existingApp = await context.PPOApplication.FindAsync(PPOApplication.Id) ?? 
                            throw new Exception($"PPO Application being edited (ID: {PPOApplication.Id}) was not found prior to updating");

                        // Save the original version for concurrency checking
                        uint originalVersion = PPOApplication.Version;

                        // Update values while preserving change tracking for auditing
                        context.Entry(existingApp).CurrentValues.SetValues(PPOApplication);

                        // Explicitly tell EF Core to use originalVersion as the concurrency token
                        // This is the critical line that makes concurrency checking work
                        context.Entry(existingApp).Property(j => j.Version).OriginalValue = originalVersion;


                        await UpdateApplicationIntents(SelectedIntents, context);
                        await context.SaveChangesAsync();
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

            CSIDE.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer, geometryValidationService);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Line String"));
            if (result.IsValid)
            {
                GeometryIsValid = true;
                if (PPOApplication is not null)
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
            using var context = contextFactory.CreateDbContext();
            var selectionPoint = new Point(coordinates[0], coordinates[1])
            {
                SRID = 27700
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

        private async Task UpdateApplicationIntents(List<int> selectedIntents, ApplicationDbContext context)
        {
            if (PPOApplication is null) return;

            // Retrieve the existing problem types for the job
            var existingIntents = await context.PPOIntents
                .Where(c => c.ApplicationId == PPOApplication.Id)
                .ToListAsync();

            // Determine the problem types to remove
            var intentsToRemove = existingIntents
                .Where(c => !selectedIntents.Contains(c.IntentId))
                .ToList();

            // Remove the entities
            context.PPOIntents.RemoveRange(intentsToRemove);

            // Determine the problem types to add
            var intentsToAdd = selectedIntents
                .Where(intentId => !existingIntents.Any(c => c.IntentId == intentId))
                .Select(intentId => new PPOIntent { IntentId = intentId, ApplicationId = PPOApplication.Id })
                .ToList();

            // Add the new problem types
            context.PPOIntents.AddRange(intentsToAdd);

            // Mark entities as unchanged if they haven't actually changed
            foreach (var existingIntent in existingIntents)
            {
                if (selectedIntents.Contains(existingIntent.IntentId))
                {
                    context.Entry(existingIntent).State = EntityState.Unchanged;
                }
            }
        }
    }
}
