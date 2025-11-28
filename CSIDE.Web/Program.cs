using ClosedXML.Excel;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using NodaTime;
using System.ComponentModel;
using System.Globalization;

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

app.MapGet("/api/ppo/export", async (HttpContext http, [FromServices] IPPOService ppoService) =>
{
    // Parse query parameters as needed
    // Fetch all results matching the current filters (no paging)
    var results = await ppoService.GetPPOApplicationsBySearchParameters(
        ParishIds: null,
        ParishId: null,
        ApplicationTypeId: null,
        ApplicationCaseStatusId: null,
        ApplicationIntentId: null,
        ApplicationPriorityId: null,
        Location: null,
        ReceivedDateFrom: null,
        ReceivedDateTo: null,
        IsPublic: null,
        OrderBy: "Id",
        OrderDirection: ListSortDirection.Descending,
        PageNumber: 1,
        PageSize: int.MaxValue,
        ct: http.RequestAborted);

    using var workbook = new ClosedXML.Excel.XLWorkbook();
    var worksheet = workbook.Worksheets.Add("PPO Applications");

    // Add headers
    worksheet.Cell(1, 1).Value = "Reference Number";
    worksheet.Cell(1, 2).Value = "Application Type";
    worksheet.Cell(1, 3).Value = "Received Date";
    worksheet.Cell(1, 4).Value = "Application Details";
    worksheet.Cell(1, 5).Value = "Parish";
    worksheet.Cell(1, 6).Value = "Case Status";

    var headerStyle = worksheet.Range("A1:F1");
    headerStyle.Style.Font.Bold = true;

    // Add data rows
    int row = 2;
    foreach (var app in results.Results)
    {
        worksheet.Cell(row, 1).Value = app.Id;
        worksheet.Cell(row, 2).Value = app.ApplicationType?.Name;
        worksheet.Cell(row, 3).Value = app.ReceivedDate?.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
        worksheet.Cell(row, 4).Value = app.ApplicationDetails;
        worksheet.Cell(row, 5).Value = string.Join(", ", app.PPOParishes.Select(p => p.Parish.Name));
        worksheet.Cell(row, 6).Value = app.CaseStatus?.Name;
        row++;
    }

    worksheet.Columns().AdjustToContents();

    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    stream.Position = 0;

    http.Response.Headers.ContentDisposition = "attachment; filename=PPOApplications.xlsx";
    http.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    await http.Response.BodyWriter.WriteAsync(stream.ToArray(), http.RequestAborted);
});

app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();