using CSIDE.Authorization;
using CSIDE.Components;
using CSIDE.Data;
using CSIDE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("CSIDE"), x =>
    {
        x.MigrationsHistoryTable("__EFMigrationsHistory", "cside");
        x.UseNodaTime();
        x.UseNetTopologySuite();
    });
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddBlazorBootstrap();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    builder.Configuration.Bind("AzureAd", options);
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = async ctxt =>
                        {
                            // Invoked before redirecting to the identity provider to authenticate. 
                            // This can be used to set ProtocolMessage.State
                            // that will be persisted through the authentication process. 
                            // The ProtocolMessage can also be used to add or customize
                            // parameters sent to the identity provider.
                            await Task.Yield();
                        },
                        OnAuthenticationFailed = async ctxt =>
                        {
                            // They tried to log in but it failed
                            await Task.Yield();
                        },
                        OnSignedOutCallbackRedirect = async ctxt =>
                        {
                            ctxt.HttpContext.Response.Redirect(ctxt.Options.SignedOutRedirectUri);
                            ctxt.HandleResponse();
                            await Task.Yield();
                        },
                        OnTicketReceived = async ctxt =>
                        {
                            await Task.Yield();
                        },
                    };
                });

builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanAccessApp", policy => policy.RequireRole("Administrator", "Ranger", "RoW Officer", "Survey Validator", "RoW Statement Editor"));

var app = builder.Build();
// add supported languages/cultures
string[] supportedCultures = ["en-GB", "cy"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
