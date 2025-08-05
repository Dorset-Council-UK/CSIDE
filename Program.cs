using CSIDE.Authorization;
using CSIDE.Components;
using CSIDE.Data;
using CSIDE.Data.Interceptors;
using CSIDE.Options;
using CSIDE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddCountrysideNetworking()
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
builder.Services.AddGovNotify(builder.Configuration);
builder.Services.AddAutoMapper(typeof(ApplicationDbContext));

builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IRightsOfWayHelperService, RightsOfWayHelperService>();
builder.Services.AddScoped<IPlacesSearchService, PlacesSearchService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGovNotifyEmailSender, GovNotifyEmailSender>();

builder.Services.AddScoped<IAuditInterceptor, AuditInterceptor>();
builder.Services.AddScoped<IRightsOfWayInterceptor, RightsOfWayInterceptor>();
builder.Services.AddScoped<IMaintenanceInterceptor, MaintenanceInterceptor>();
builder.Services.AddScoped<IInfrastructureInterceptor, InfrastructureInterceptor>();
builder.Services.AddScoped<ILandownerDepositInterceptor, LandownerDepositInterceptor>();
builder.Services.AddScoped<IDMMOInterceptor, DMMOInterceptor>();
builder.Services.AddScoped<IPPOInterceptor, PPOInterceptor>();
builder.Services.AddScoped<ISurveyInterceptor, SurveyInterceptor>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddSingleton<IMapLinkHelperService, MapLinkHelperService>();

builder.Services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);
builder.Services.AddSingleton<IAuthorizationHandler, SurveyAuthorizationHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthenticatedOnly", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("IsSurveyor", policy => policy.Requirements.Add(new IsSurveyorRequirement()))
    .AddDefaultPolicy("CanAccessApp", policy => policy.RequireRole("Administrator", "Ranger", "RoW Officer", "Survey Validator", "RoW Statement Editor", "View", "Surveyor"));

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