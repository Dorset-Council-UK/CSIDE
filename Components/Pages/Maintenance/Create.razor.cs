using BlazorBootstrap;
using CSIDE.Components.Maintenance;
using CSIDE.Components.Mapping;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;
using NetTopologySuite.Features;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CSIDE.Components.Pages.Maintenance
{
    public partial class Create(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Create> logger)
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        private List<BreadcrumbItem>? NavItems;
        private Modal routeValidationModal = default!;
        private Modal errorModal = default!;

        private CreateMap? createMap;

        private Job? Job { get; set; }
        private JobStatus[]? JobStatuses { get; set; }
        private JobPriority[]? JobPriorities { get; set; }
        private ProblemType[]? ProblemTypes { get; set; }

        private bool IsBusy { get; set; }
        private string? ErrorMessage { get; set; }
        private bool CompleteDateShown { get; set; } = false;
        private List<int> SelectedProblemTypes { get; set; } = [];

        private JobEditForm? childJobEditForm;

        protected override async Task OnInitializedAsync()
        {
            NavItems = new List<BreadcrumbItem>
        {
            new BreadcrumbItem{ Text = localizer["Home Title"], Href ="/" },
            new BreadcrumbItem{ Text = localizer["Maintenance Title"], Href="/Maintenance" },
            new BreadcrumbItem{ Text = localizer["Maintenance Create Title"], IsCurrentPage = true }
        };

            using var context = contextFactory.CreateDbContext();
            JobStatuses = await context.MaintenanceJobStatuses.OrderBy(s => s.SortOrder).ToArrayAsync();
            JobPriorities = await context.MaintenanceJobPriorities.OrderBy(p => p.SortOrder).ToArrayAsync();
            ProblemTypes = await context.ProblemTypes.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
            Job = new()
            {
                JobStatusId = JobStatuses.FirstOrDefault()?.Id,
                JobPriorityId = JobPriorities.FirstOrDefault()?.Id
            };
        }


        private async Task SubmitFormAsync()
        {
            if (IsBusy)
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

                        context.MaintenanceJobs.Add(Job);

                        await context.SaveChangesAsync();
                        CreateMaintenanceProblemTypes(SelectedProblemTypes, Job.Id, context);
                        await context.SaveChangesAsync();
                        //redirect
                        navigationManager.NavigateTo($"/Maintenance/Details/{Job.Id}");
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = localizer["Save Error Message"];
                    logger.LogError(ex, "An error occurred creating a maintenance job");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private static void CreateMaintenanceProblemTypes(List<int> selectedProblemTypes, int JobId, ApplicationDbContext context)
        {
            //add new problem types
            foreach (int problemType in selectedProblemTypes)
            {
                context.MaintenanceJobProblemTypes.Add(new JobProblemType { ProblemTypeId = problemType, JobId = JobId });
            }
            return;
        }


        //TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)

        private async Task ValidateGeometry(string features)
        {
            //check the geometry is a single point and is on a valid route
            GeoJsonReader _geoJsonReader = new GeoJsonReader();
            FeatureCollection featureCollection = _geoJsonReader.Read<FeatureCollection>(features);

            CSIDE.Validators.Geometry.GeometryValidator validator = new(contextFactory, localizer);

            var result = await validator.ValidateAsync(featureCollection, options => options.IncludeRuleSets("Single Point", "Point On Route"));
            if (result.IsValid)
            {
                //get route code and update Job.Geom
                if (Job is not null)
                {
                    Job.Geom = featureCollection.First().Geometry.Centroid;
                    Job.Geom.SRID = 27700;
                    using var context = contextFactory.CreateDbContext();
                    //TODO - Move this to shared location
                    var Route = await context.Routes.Where(r => r.Geom.Distance(Job.Geom) < 20).OrderBy(r => r.Geom.Distance(Job.Geom)).FirstOrDefaultAsync();
                    if (Route is not null)
                    {
                        Job.RouteId = Route.RouteCode;
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
            await createMap!.ClearDrawnGeometries();
            await routeValidationModal.HideAsync();
            await errorModal.HideAsync();
        }
    }
}
