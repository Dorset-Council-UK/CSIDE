using CSIDE.API.Endpoints.DMMO;
using CSIDE.API.Endpoints.LandownerDeposits;
using CSIDE.API.Endpoints.Maintenance;
using CSIDE.API.Endpoints.PPO;
using CSIDE.API.Models;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class ApiEndpointsExtensions
{
    internal static void MapApiEndpoints(this WebApplication app)
    {
        MapDMMOApplicationEndpoints(app);
        MapPPOApplicationEndpoints(app);
        MapLandownerDepositEndpoints(app);
        MapMaintenanceJobEndpoints(app);
    }

    private static void MapDMMOApplicationEndpoints(WebApplication app)
    {
        var group = app
            .NewVersionedApi()
            .HasApiVersion(Versions.V1.Version)
            .MapGroup("/api/dmmo")
            .WithTags("dmmo");

        // Get all DMMO applications
        group
            .MapGet("/all", DMMOApplicationEndpoints.GetAllPublicDMMOApplications)
            .WithSummary("DMMO Applications - all")
            .WithDescription("Get all public DMMO applications from the database");

        // Get a specific DMMO application by its ID
        group
            .MapGet("/{id}", DMMOApplicationEndpoints.GetDMMOApplicationById)
            .WithSummary("DMMOApplications - by ID")
            .WithDescription("Get a specific DMMO application by its ID");

        // Search for DMMO applications based on various parameters
        group
            .MapPost("/search", DMMOApplicationEndpoints.GetDMMOApplicationsBySearchParameters)
            .WithSummary("DMMO Applications - search")
            .WithDescription("Search for DMMO applications based on various parameters");
    }

    private static void MapPPOApplicationEndpoints(WebApplication app)
    {
        var group = app
            .NewVersionedApi()
            .HasApiVersion(Versions.V1.Version)
            .MapGroup("/api/ppo")
            .WithTags("ppo");

        // Get all PPO applications
        group
            .MapGet("/all", PPOApplicationEndpoints.GetAllPPOApplications)
            .WithSummary("PPO Applications - all")
            .WithDescription("Get all PPO applications from the database");

        // Get a specific PPO application by its ID
        group
            .MapGet("/{id}", PPOApplicationEndpoints.GetPPOApplicationById)
            .WithSummary("PPO Applications - by ID")
            .WithDescription("Get a specific PPO application by its ID");

        // Search for PPO applications based on various parameters
        group
            .MapPost("/search", PPOApplicationEndpoints.GetPPOApplicationsBySearchParameters)
            .WithSummary("PPO Applications - search")
            .WithDescription("Search for PPO applications based on various parameters");
    }

    private static void MapLandownerDepositEndpoints(WebApplication app)
    {
        var group = app
            .NewVersionedApi()
            .HasApiVersion(Versions.V1.Version)
            .MapGroup("/api/landowner-deposits")
            .WithTags("landownerdeposits");

        // Get all Landowner Deposits
        group
            .MapGet("/all", LandownerDepositEndpoints.GetAllLandownerDeposits)
            .WithSummary("Landowner Deposits - all")
            .WithDescription("Get all Landowner Deposits from the database");

        // Get a specific Landowner Deposit by its ID
        group
            .MapGet("/{id}", LandownerDepositEndpoints.GetLandownerDepositById)
            .WithSummary("Landowner Deposits - by ID")
            .WithDescription("Get a specific Landowner Deposit by its ID");

        // Search for Landowner Deposits based on various parameters
        group
            .MapPost("/search", LandownerDepositEndpoints.GetLandownerDepositsBySearchParameters)
            .WithSummary("Landowner Deposits - search")
            .WithDescription("Search for Landowner Deposits based on various parameters");
    }

    private static void MapMaintenanceJobEndpoints(WebApplication app)
    {
        var group = app
            .NewVersionedApi()
            .HasApiVersion(Versions.V1.Version)
            .MapGroup("/api/maintenance-jobs")
            .WithTags("maintenancejobs");

        // Get a specific maintenance job by its ID
        group
            .MapGet("/{id}", MaintenanceJobEndpoints.GetMaintenanceJobById)
            .WithSummary("Maintenance Jobs - by ID")
            .WithDescription("Get a specific maintenance job by its ID");

        // Search for maintenance jobs based on various parameters
        group
            .MapGet("/search", MaintenanceJobEndpoints.GetMaintenanceJobsBySearchParameters)
            .WithSummary("Maintenance Jobs - search")
            .WithDescription("Search for maintenance jobs based on various parameters");

        //create new maintenance job - secured endpoint
        group
            .MapPost("/create", MaintenanceJobEndpoints.CreateMaintenanceJob)
            .RequireAuthorization("ApiKeyPolicy")
            .WithSummary("Create Maintenance Job")
            .WithDescription("Create a new maintenance job - key required");

        group
            .MapPost("/add-media/{id}", MaintenanceJobEndpoints.AddMediaToJob)
            .RequireAuthorization("ApiKeyPolicy")
            .WithSummary("Add Media to Job")
            .WithDescription("Add a file to a maintenance job")
            .DisableAntiforgery();

        // Add subscription to maintenance job
        group
            .MapPost("/subscriptions/add/{id}", MaintenanceJobEndpoints.AddSubscriptionToJob)
            .WithSummary("Subscribe to Maintenance Job Updates")
            .WithDescription("Sign up to receive email notifications for a specific maintenance job");
    }
}
