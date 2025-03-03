using BlazorBootstrap;
using CSIDE.Components.Maintenance;
using CSIDE.Components.Mapping;
using CSIDE.Data;
using CSIDE.Data.Models.Maintenance;
using CSIDE.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System.Globalization;

namespace CSIDE.Components.Pages.Maintenance;

public partial class Create(IDbContextFactory<ApplicationDbContext> contextFactory, NavigationManager navigationManager, ILogger<Create> logger, IRightsOfWayHelperService geometryValidationService)
{
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
        NavItems = [
            new() { Text = localizer["Home Title"], Href ="/" },
            new() { Text = localizer["Maintenance Title"], Href="Maintenance" },
            new() { Text = localizer["Maintenance Create Title"], IsCurrentPage = true },
        ];

        await using var context = contextFactory.CreateDbContext();
        JobStatuses = await context.MaintenanceJobStatuses.OrderBy(s => s.SortOrder).ToArrayAsync();
        JobPriorities = await context.MaintenanceJobPriorities.OrderBy(p => p.SortOrder).ToArrayAsync();
        ProblemTypes = await context.ProblemTypes.AsNoTracking().OrderBy(p => p.Name).ToArrayAsync();
        Job = new()
        {
            JobStatusId = JobStatuses?.FirstOrDefault()?.Id,
            JobPriorityId = JobPriorities?.FirstOrDefault()?.Id,
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
                    navigationManager.NavigateTo($"Maintenance/Details/{Job.Id}");
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

    /// <summary>
    ///     <para>Validate the geometry of the drawn feature</para>
    ///     <para>TODO - The following is reused in Edit.razor, so should be shared somewhere (unless they diverge significantly)</para>
    /// </summary>
    private async Task ValidateGeometry(string features)
    {
        //check the geometry is a single point and is on a valid route
        GeoJsonReader _geoJsonReader = new GeoJsonReader();
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
            if (result.Errors.Exists(
                failure => string.Equals(failure.ErrorCode, "GEOM_OUTSIDE_BOUNDS", StringComparison.Ordinal)) ||
                result.Errors.Exists(failure => string.Equals(failure.ErrorCode, "INVALID_GEOM", StringComparison.Ordinal))
                )
            {
                //show generic error
                await ShowGeometryValidationErrorModal();
            }
            // if its valid, check to see if there was no route nearby
            // NOTE I know it seems backwards to test this way round, but if you don't, 
            // invalid geometries always come back saying 'no route found', which is not the right message
            // TODO - Improve logic through use of conditional validation
            else if (result.Errors.Exists(failure => string.Equals(failure.ErrorCode, "NO_ROUTE_NEARBY", StringComparison.Ordinal)))
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

    /// <summary>Generate a key attribute for the JobEditForm component to help with efficient re-renders when data changes</summary>
    /// <remarks>This can return any type, I went with string for clarity</remarks>
    private string CreateJobKey()
    {
        // When the job is not initialised, use a simple component key
        if (Job is null)
        {
            return "MaintenanceCreateJob";
        }

        // When the job is new, return a unique component key
        if (Job.Id == 0)
        {
            return $"MaintenanceCreateJob-New-{Job.GetHashCode().ToString(CultureInfo.InvariantCulture)}";
        }

        // When the job is initialised, return a component key based on the job ID
        return $"MaintenanceCreateJob-{Job.Id.ToString(CultureInfo.InvariantCulture)}";
    }
}
