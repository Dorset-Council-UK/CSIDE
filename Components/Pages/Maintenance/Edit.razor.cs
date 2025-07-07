using BlazorBootstrap;
using CSIDE.Components.Maintenance;
using CSIDE.Components.Mapping;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using NetTopologySuite.Features;
using FluentValidation;
using CSIDE.Services;

namespace CSIDE.Components.Pages.Maintenance
{
    public partial class Edit(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Edit> logger, IRightsOfWayHelperService geometryValidationService)
    {
        [Parameter]
        public int JobId { get; set; }

        private Job? Job { get; set; }
        private JobStatus[]? JobStatuses { get; set; }
        private JobPriority[]? JobPriorities { get; set; }
        private Team[]? MaintenanceTeams { get; set; }
        private ProblemType[]? ProblemTypes { get; set; }

        private JobEditForm? childJobEditForm;
        private List<int> SelectedProblemTypes { get; set; } = [];

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
                new BreadcrumbItem{ Text = localizer["Maintenance Title"], Href="Maintenance" },
                new BreadcrumbItem{ Text = localizer["Maintenance Details Title", JobId], Href=$"Maintenance/Details/{JobId}"},
                new BreadcrumbItem{ Text = localizer["Maintenance Edit Title", JobId], IsCurrentPage = true }
            ];
            IsBusy = true;
            try
            {
                using var context = contextFactory.CreateDbContext();
                JobStatuses = await context.MaintenanceJobStatuses.AsNoTracking().OrderBy(s => s.SortOrder).ToArrayAsync();
                JobPriorities = await context.MaintenanceJobPriorities.AsNoTracking().OrderBy(p => p.SortOrder).ToArrayAsync();
                MaintenanceTeams = await context.MaintenanceTeams.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                ProblemTypes = await context.ProblemTypes.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
                Job = await context.MaintenanceJobs.IgnoreAutoIncludes().Include(j => j.ProblemTypes).FirstOrDefaultAsync(j => j.Id == JobId);
                SelectedProblemTypes = Job!.ProblemTypes.Select(j => j.ProblemTypeId).ToList();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SubmitFormAsync()
        {
            if (Job is null || IsBusy)
            {
                ErrorMessage = null;
                return;
            }
            if (await childJobEditForm!.ValidateAsync())
            {
                IsBusy = true;
                StateHasChanged();
                try
                {
                    if (Job is not null)
                    {
                        using var context = contextFactory.CreateDbContext();

                        // Get the existing entity with tracking enabled
                        var existingJob = await context.MaintenanceJobs.FindAsync(Job.Id)
                            ?? throw new Exception($"Maintenance job being edited (ID: {Job.Id}) was not found prior to updating");

                        // Save the original version for concurrency checking
                        uint originalVersion = Job.Version;

                        // Update values while preserving change tracking for auditing
                        context.Entry(existingJob).CurrentValues.SetValues(Job);

                        // Explicitly tell EF Core to use originalVersion as the concurrency token
                        // This is the critical line that makes concurrency checking work
                        context.Entry(existingJob).Property(j => j.Version).OriginalValue = originalVersion;

                        await UpdateMaintenanceProblemTypes(SelectedProblemTypes, context);
                        await context.SaveChangesAsync();

                        //redirect
                        NavigateBackToJobDetailsPage();
                        
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ErrorMessage = localizer["Concurrency Error Message", localizer["Maintenance Details Title", Job.Id]];
                    logger.LogWarning(ex, "A concurrency conflict occurred when updating a maintenance job");
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred updating a maintenance job");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task UpdateMaintenanceProblemTypes(List<int> selectedProblemTypes, ApplicationDbContext context)
        {
            if (Job is null) return;

            // Retrieve the existing problem types for the job
            var existingProblemTypes = await context.MaintenanceJobProblemTypes
                .Where(c => c.JobId == Job.Id)
                .ToListAsync();

            // Determine the problem types to remove
            var problemTypesToRemove = existingProblemTypes
                .Where(c => !selectedProblemTypes.Contains(c.ProblemTypeId))
                .ToList();

            // Remove the entities
            context.MaintenanceJobProblemTypes.RemoveRange(problemTypesToRemove);

            // Determine the problem types to add
            var problemTypesToAdd = selectedProblemTypes
                .Where(problemTypeId => !existingProblemTypes.Any(c => c.ProblemTypeId == problemTypeId))
                .Select(problemTypeId => new JobProblemType { ProblemTypeId = problemTypeId, JobId = Job.Id })
                .ToList();

            // Add the new problem types
            context.MaintenanceJobProblemTypes.AddRange(problemTypesToAdd);

            // Mark entities as unchanged if they haven't actually changed
            foreach (var existingProblemType in existingProblemTypes)
            {
                if (selectedProblemTypes.Contains(existingProblemType.ProblemTypeId))
                {
                    context.Entry(existingProblemType).State = EntityState.Unchanged;
                }
            }
        }


        private void NavigateBackToJobDetailsPage()
        {
            navigationManager.NavigateTo($"Maintenance/Details/{JobId}");
        }

        //TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)

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
                if (Job is not null)
                {
                    Job.Geom = featureCollection.First().Geometry.Centroid;
                    Job.Geom.SRID = 27700;

                    var NearestRoute = await geometryValidationService.GetNearestRouteAsync(Job.Geom);
                    if (NearestRoute is not null)
                    {
                        Job.RouteId = NearestRoute.RouteCode;
                    }
                }
            }
            else
            {
                // Check to see what the error is and show appropriate error message
                // first check if the geometry was invalid
                if (result.Errors.Any(
                    failure => string.Equals(failure.ErrorCode, "GEOM_OUTSIDE_BOUNDS", StringComparison.Ordinal)) || 
                    result.Errors.Any(failure => string.Equals(failure.ErrorCode, "INVALID_GEOM", StringComparison.Ordinal))
                    )
                {
                    //show generic error
                    await ShowGeometryValidationErrorModal();
                }
                // if its valid, check to see if there was no route nearby
                // NOTE I know it seems backwards to test this way round, but if you don't,
                // invalid geometries always come back saying 'no route found', which is not the right message
                // TODO - Improve logic through use of conditional validation
                else if (result.Errors.Any(failure => string.Equals(failure.ErrorCode, "NO_ROUTE_NEARBY", StringComparison.Ordinal)))
                {
                    //we assigned the geom at this point in case the user goes ahead with overriding the route ID
                    if (Job is not null)
                    {
                        Job.Geom = featureCollection.First().Geometry.Centroid;
                        Job.Geom.SRID = 27700;
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
            if (Job is not null)
            {
                Job.Geom = null;
                Job.RouteId = null;
            }
            await editMap!.ClearDrawnGeometries();
            await routeValidationModal.HideAsync();
            await errorModal.HideAsync();
        }
    }
}
