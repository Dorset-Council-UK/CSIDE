using CSIDE.Authorization;
using CSIDE.Data;
using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using CSIDE.Shared.Services;
using CSIDE.Web.Authorization;
using CSIDE.Web.Components;
using CSIDE.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddCountrysideNetworking()
    .AddCountrysideOptions()
    .AddCountrysideAzureKeyVault()
    .AddCountrysideAuthentication()
    .AddCountrysideDatabase()
    .AddCountrysideTelemetry();

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

// Services from data project
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IDMMOService, DMMOService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<IGovNotifyEmailSender, GovNotifyEmailSender>();
builder.Services.AddScoped<IInfrastructureService, InfrastructureService>();
builder.Services.AddScoped<ILandownerDepositService, LandownerDepositService>();
builder.Services.AddScoped<IMaintenanceJobsService, MaintenanceJobsService>();
builder.Services.AddSingleton<IMapLinkHelperService, MapLinkHelperService>();
builder.Services.AddScoped<IPlacesSearchService, PlacesSearchService>();
builder.Services.AddScoped<IPPOService, PPOService>();
builder.Services.AddScoped<IRightsOfWayService, RightsOfWayService>();
builder.Services.AddScoped<ISharedDataService, SharedDataService>();
builder.Services.AddScoped<IUserService, UserService>();

// Current user service to allow the audit interceptor to use user details
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();

builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);
builder.Services.AddSingleton<IAuthorizationHandler, SurveyAuthorizationHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthenticatedOnly", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("IsSurveyor", policy => policy.Requirements.Add(new IsSurveyorRequirement()))
    .AddDefaultPolicy("CanAccessApp", policy => policy.RequireRole("Administrator", "Ranger", "RoW Officer", "Survey Validator", "RoW Statement Editor", "View", "Surveyor"));

var options = builder.Configuration.GetSection(CSIDEOptions.SectionName).Get<CSIDEOptions>();

var app = builder.Build();

app.UsePathBase($"/{options?.PathBase}");

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

if (options is not null && options.UseHttpsRedirection)
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