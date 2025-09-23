using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using Microsoft.AspNetCore.Http.Json;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args);

// Add problem details
builder.Services.AddProblemDetails();

// Add health checks
builder.Services.AddApiHealthChecks();

// Add versioning
builder.Services.AddVersioning();

// Add OpenAPI/Swagger
builder.Services.AddOpenApiSwagger();

// Add db, options, telemetry and key vault
builder
    .AddCountrysideNetworking()
    .AddCountrysideOptions()
    .AddCountrysideTelemetry()
    .AddCountrysideAzureKeyVault()
    .AddCountrysideDatabase();

// Services from data project
builder.Services.AddHttpClient(); // needed for PlacesSearchService
builder.Services.AddScoped<IDMMOService, DMMOService>();
builder.Services.AddScoped<ILandownerDepositService, LandownerDepositService>();
builder.Services.AddScoped<IMaintenanceJobsService, MaintenanceJobsService>();
builder.Services.AddScoped<IPlacesSearchService, PlacesSearchService>();
builder.Services.AddScoped<IPPOService, PPOService>();

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
    // Map Swagger UI
    app.MapSwaggerUI();
}

if (options is not null && options.UseHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

// Map health checks
app.MapApiHealthChecks();

// Map endpoints
app.MapApiEndpoints();

app.Run();