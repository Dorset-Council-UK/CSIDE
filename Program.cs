using CSIDE.Authorization;
using CSIDE.Components;
using CSIDE.Options;
using CSIDE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddResilience()
    .AddCountrysideOptions()
    .AddCountrysideTelemetry()
    .AddCountrysideAzureKeyVault()
    .AddCountrysideAuthentication()
    .AddCountrysideDatabase();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddBlazorBootstrap();

builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IRightsOfWayHelperService, RightsOfWayHelperService>();
builder.Services.AddScoped<IPlacesSearchService, PlacesSearchService>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthenticatedOnly", policy => policy.RequireAuthenticatedUser())
    .AddDefaultPolicy("CanAccessApp", policy => policy.RequireRole("Administrator", "Ranger", "RoW Officer", "Survey Validator", "RoW Statement Editor", "View"));

var options = builder.Configuration.GetSection(CSIDEOptions.SectionName).Get<CSIDEOptions>();

var app = builder.Build();

app.UsePathBase($"/{options.PathBase}");

app.UseForwardedHeaders();

// add supported languages/cultures
string[] supportedCultures = ["en-GB", "cy"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

if (options.UseHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();