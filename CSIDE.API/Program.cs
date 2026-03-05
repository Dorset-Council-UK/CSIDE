using CSIDE.API.Authorization;
using CSIDE.API.Services;
using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using CSIDE.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NetTopologySuite.IO.Converters;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add problem details
builder.Services.AddProblemDetails();

// Add health checks
builder.Services.AddApiHealthChecks();

// Add versioning
builder.Services.AddVersioning();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.WithMethods("POST", "GET");
            policy.WithHeaders("X-API-Key");
        });
});

// Add OpenAPI/Swagger
OpenApiExtensions.AddOpenApi(builder.Services);

// Add db, options, telemetry and key vault
builder
    .AddCountrysideNetworking()
    .AddCountrysideOptions()
    .AddCountrysideAzureKeyVault()
    .AddCountrysideDatabase()
    .AddCountrysideTelemetry();

// Services from data project
builder.Services.AddHttpClient(); // needed for PlacesSearchService
builder.Services.AddMemoryCache();
builder.Services.AddLocalization();
builder.Services.AddGovNotify(builder.Configuration);

builder.Services.AddScoped<IDMMOService, DMMOService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<ILandownerDepositService, LandownerDepositService>();
builder.Services.AddScoped<IMaintenanceJobsService, MaintenanceJobsService>();
builder.Services.AddScoped<IRightsOfWayService, RightsOfWayService>();
builder.Services.AddScoped<IPlacesSearchService, PlacesSearchService>();
builder.Services.AddScoped<IPPOService, PPOService>();
builder.Services.AddScoped<ISharedDataService, SharedDataService>();
builder.Services.AddScoped<IGovNotifyEmailSender, GovNotifyEmailSender>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

//file size limits
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});

//auth
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKeyPolicy", policy =>
    {
        policy.AddAuthenticationSchemes(ApiKeyAuthenticationHandler.SchemeName);
        policy.RequireAuthenticatedUser();
    });
});

builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName, null);

// json serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new GeoJsonConverterFactory());
});

var options = builder.Configuration.GetSection(CSIDEOptions.SectionName).Get<CSIDEOptions>();

var app = builder.Build();

app.UseForwardedHeaders();

app.UsePathBase($"/{options?.PathBase}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapGet("/swagger", () => Results.Redirect("scalar", permanent: false));

    // Map Scalar
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.Kepler;
        options.Title = "CSIDE API Reference - Scalar";
        options.DefaultOpenAllTags = true;
        options.HideClientButton = true;
    });
}

if (options is not null && options.UseHttpsRedirection)
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseCors();
app.UseStatusCodePages();
// Map health checks
app.MapApiHealthChecks();

// Map endpoints
app.MapApiEndpoints();

app.Run();