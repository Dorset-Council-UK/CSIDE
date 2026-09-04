using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using CSIDE.Data;
using CSIDE.Data.Models.Audit;
using CSIDE.Data.Models.Surveys;
using CSIDE.Data.Services;
using CSIDE.Shared.Options;
using CSIDE.Web.Authorization;
using CSIDE.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using NodaTime;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.AspNetCore.Builder;
#pragma warning restore IDE0130 // Namespace does not match folder structure

internal static class WebApplicationBuilderExtension
{
    /// <summary>
    /// Add resilience to the HttpClient
    /// </summary>
    internal static WebApplicationBuilder AddCountrysideNetworking(this WebApplicationBuilder builder)
    {
        //add reslience handlers to http client
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();
        });

        //add optional forwarded headers middleware handler
        var section = builder.Configuration
            .GetSection(CSIDEOptions.SectionName)
            .GetSection(NetworkingOptions.SectionName);
        var networkingOptions = section.Get<NetworkingOptions>();

        if(networkingOptions is not null && networkingOptions.UseForwardedHeadersMiddleware)
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;

                if (networkingOptions.KnownProxies is not null)
                {
                    foreach (var proxy in networkingOptions.KnownProxies)
                    {
                        if (IPAddress.TryParse(proxy, out var ipAddress))
                        {
                            options.KnownProxies.Add(ipAddress);
                        }
                    }
                }
            });
        }
        

        return builder;
    }

    /// <summary>
    /// Add all the Countryside options
    /// </summary>
    internal static WebApplicationBuilder AddCountrysideOptions(this WebApplicationBuilder builder)
    {
        var sectionCSIDE = builder.Configuration.GetSection(CSIDEOptions.SectionName);
        var sectionApplicationInsights = sectionCSIDE.GetSection(ApplicationInsightsOptions.SectionName);
        var sectionMapping = sectionCSIDE.GetSection(MappingOptions.SectionName);
        var sectionKeyVault = sectionCSIDE.GetSection(KeyVaultOptions.SectionName);
        var sectionTheme = sectionCSIDE.GetSection(ThemeOptions.SectionName);
        var sectionAzureBlobStorage = sectionCSIDE.GetSection(AzureBlobStorageOptions.SectionName);
        var sectionNetworking = sectionCSIDE.GetSection(NetworkingOptions.SectionName);
        var sectionIDPrefixes = sectionCSIDE.GetSection(IDPrefixOptions.SectionName);
        var sectionDatabase = sectionCSIDE.GetSection(DatabaseOptions.SectionName);

        builder.Services
            .Configure<CSIDEOptions>(sectionCSIDE)
            .Configure<ApplicationInsightsOptions>(sectionApplicationInsights)
            .Configure<MappingOptions>(sectionMapping)
            .Configure<KeyVaultOptions>(sectionKeyVault)
            .Configure<ThemeOptions>(sectionTheme)
            .Configure<AzureBlobStorageOptions>(sectionAzureBlobStorage)
            .Configure<NetworkingOptions>(sectionNetworking)
            .Configure<IDPrefixOptions>(sectionIDPrefixes)
            .Configure<DatabaseOptions>(sectionDatabase);

        return builder;
    }

    /// <summary>
    /// Add Application Insights telemetry if a connection string is provided
    /// </summary>
    internal static WebApplicationBuilder AddCountrysideTelemetry(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration
            .GetSection(CSIDEOptions.SectionName)
            .GetSection(ApplicationInsightsOptions.SectionName);

        var applicationInsightsOptions = section.Get<ApplicationInsightsOptions>();

        if (string.IsNullOrWhiteSpace(applicationInsightsOptions?.ConnectionString))
        {
            return builder;
        }

        builder.Services
            .AddOpenTelemetry()
            .UseAzureMonitor(options => {
                options.ConnectionString = applicationInsightsOptions.ConnectionString;
            });

        return builder;
    }

    /// <summary>
    /// Set up Azure Key Vault if a KeyVault name is provided in the configuration
    /// </summary>
    internal static WebApplicationBuilder AddCountrysideAzureKeyVault(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration
            .GetSection(CSIDEOptions.SectionName)
            .GetSection(KeyVaultOptions.SectionName);

        var keyVaultOptions = section.Get<KeyVaultOptions>();

        if (string.IsNullOrWhiteSpace(keyVaultOptions?.Name))
        {
            return builder;
        }

        using var x509Store = new X509Store(StoreLocation.LocalMachine);
        x509Store.Open(OpenFlags.ReadOnly);

        var x509Certificate = x509Store.Certificates
            .Find(X509FindType.FindByThumbprint, keyVaultOptions.AzureAd.CertificateThumbprint, validOnly: false)
            .OfType<X509Certificate2>()
            .Single();

        builder.Configuration.AddAzureKeyVault(
            new Uri($"https://{keyVaultOptions.Name}.vault.azure.net/"),
            new ClientCertificateCredential(keyVaultOptions.AzureAd.DirectoryId, keyVaultOptions.AzureAd.ApplicationId, x509Certificate));

        return builder;
    }

    /// <summary>
    /// Add the Microsoft Identity Web App authentication
    /// </summary>
    internal static WebApplicationBuilder AddCountrysideAuthentication(this WebApplicationBuilder builder)
    {
        const string openIdConnectClientName = "OpenIDConnectResilient";
        const string graphClientName = "MicrosoftGraphResilient";

        var azureAdSection = builder.Configuration
            .GetSection(CSIDEOptions.SectionName)
            .GetSection(AzureAdOptions.SectionName);

        // Register named HttpClients for external auth calls with resilience
        builder.Services
            .AddHttpClient(openIdConnectClientName)
            .AddStandardResilienceHandler();

        builder.Services
            .AddHttpClient(graphClientName, httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://graph.microsoft.com/");
            })
            .AddStandardResilienceHandler();

        // Add microsoft identity web app authentication
        builder.Services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                azureAdSection.Bind(options);
                options.ResponseType = "code";

                if (string.IsNullOrWhiteSpace(options.SignedOutCallbackPath))
                {
                    options.SignedOutCallbackPath = "/signout-callback-oidc";
                }

                options.ErrorPath = "/Error";
                options.SignedOutRedirectUri = "/account/signedout";
                options.AccessDeniedPath = "/account/accessdenied";
            });
        builder.Services.AddRazorPages();
        builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

        // Configure OpenIdConnectOptions to use our resilient HttpClient
        builder.Services
            .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<IHttpClientFactory>((options, httpClientFactory) =>
            {
                options.Backchannel = httpClientFactory.CreateClient(openIdConnectClientName);
                options.SignedOutRedirectUri = "/account/signedout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.Events ??= new OpenIdConnectEvents();
                var existingRedirectHandler = options.Events.OnRedirectToIdentityProvider;
                var existingOnRemoteFailureHandler = options.Events.OnRemoteFailure;
                var existingOnTokenValidatedHandler = options.Events.OnTokenValidated;

                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    if (existingRedirectHandler != null)
                        await existingRedirectHandler(context);

                    context.ProtocolMessage.Prompt = "select_account";

                    if (context.Properties.Items.TryGetValue(AuthenticationContextConstants.StepUpAcrValuesItemKey, out var acrValues)
                        && !string.IsNullOrWhiteSpace(acrValues))
                    {
                        context.ProtocolMessage.AcrValues = acrValues;
                    }

                    if (context.Properties.Items.TryGetValue(AuthenticationContextConstants.StepUpClaimsItemKey, out var claimsChallenge)
                        && !string.IsNullOrWhiteSpace(claimsChallenge))
                    {
                        context.ProtocolMessage.SetParameter("claims", claimsChallenge);
                    }
                };
                // Workaround for Entra External ID stale session errors on first login of the day.
                // When a user's Entra session expires overnight, the first authentication attempt can fail
                // with AADSTS50133 (session invalid due to expiry) or AADSTS165000 (session context missing).
                // This handler retries authentication once to obtain a fresh session. If the retry also fails
                // (indicating a genuine issue like a required password change), the user is redirected to an
                // error page to avoid an infinite redirect loop.
                options.Events.OnRemoteFailure = async context =>
                {
                    if (context.Failure?.Message?.Contains("AADSTS50133", StringComparison.Ordinal) == true ||
                        context.Failure?.Message?.Contains("AADSTS165000", StringComparison.Ordinal) == true)
                    {
                        const string authRetryCookieName = "authretry";
                        var hasRetried = context.Request.Cookies.ContainsKey(authRetryCookieName);

                        if (!hasRetried)
                        {
                            context.Response.Cookies.Append(authRetryCookieName, "1", new CookieOptions
                            {
                                HttpOnly = true,
                                IsEssential = true,
                                Secure = true,
                                SameSite = SameSiteMode.Lax,
                                MaxAge = TimeSpan.FromMinutes(5)
                            });

                            var signInPath = context.Request.PathBase.Add("/MicrosoftIdentity/Account/SignIn");
                            context.Response.Redirect($"{signInPath}?returnUrl=%2F");
                            context.HandleResponse();
                        }
                        else
                        {
                            context.Response.Cookies.Delete(authRetryCookieName);
                            var loginFailedPath = context.Request.PathBase.Add("/Account/LoginFailed");
                            context.Response.Redirect(loginFailedPath);
                            context.HandleResponse();
                        }
                    }
                    else
                    {

                        if (existingOnRemoteFailureHandler != null)
                            await existingOnRemoteFailureHandler(context);
                    }
                };

                options.Events.OnTokenValidated = async context =>
                {
                    if (existingOnTokenValidatedHandler != null)
                    {
                        await existingOnTokenValidatedHandler(context);
                    }

                    if (!context.Properties.Items.TryGetValue(AuthenticationContextConstants.StepUpAcrValuesItemKey, out var stepUpAcrValues)
                        || !context.Properties.Items.ContainsKey(AuthenticationContextConstants.StepUpClaimsItemKey)
                        || !string.Equals(stepUpAcrValues, AuthenticationContextConstants.ManagementMfa, StringComparison.OrdinalIgnoreCase)
                        || context.Principal is null
                        || !context.Principal.HasAuthenticationContext(AuthenticationContextConstants.ManagementMfa))
                    {
                        return;
                    }

                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("AuthenticationStepUpAudit");

                    var userId = context.Principal.GetObjectId() ?? "Unknown";
                    var userName = context.Principal.FindFirst("name")?.Value
                        ?? context.Principal.Identity?.Name
                        ?? "Unknown";

                    var auditLog = new AuditLog
                    {
                        EntityName = "Authentication",
                        EntityId = userId,
                        ChangeType = "StepUpSucceeded",
                        UserId = userId,
                        UserName = userName,
                        LogDate = SystemClock.Instance.GetCurrentInstant(),
                        NewValues = JsonSerializer.SerializeToDocument(new
                        {
                            AuthenticationContext = AuthenticationContextConstants.ManagementMfa,
                            Acrs = context.Principal.FindAll("acrs").Select(x => x.Value).ToArray(),
                            Amr = context.Principal.FindAll("amr").Select(x => x.Value).ToArray(),
                        }),
                    };

                    try
                    {
                        var auditLogService = context.HttpContext.RequestServices.GetRequiredService<IAuditLogService>();
                        await auditLogService.AddLogAsync(auditLog, context.HttpContext.RequestAborted);
                    }
                    catch (DbUpdateException dbUpdateException)
                    {
                        logger.LogError(dbUpdateException, "Failed to write step-up authentication audit log for user {UserId}", userId);
                    }
                    catch (InvalidOperationException invalidOperationException)
                    {
                        logger.LogError(invalidOperationException, "Failed to resolve audit logging services for step-up authentication user {UserId}", userId);
                    }
                    catch (OperationCanceledException) when (context.HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        // Request was aborted; don't fail authentication because audit logging was cancelled.
                    }
                    catch (SystemException systemException)
                    {
                        logger.LogError(systemException, "Unexpected system error writing step-up authentication audit log for user {UserId}", userId);
                    }
                };
            });

        builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.AccessDeniedPath = "/account/accessdenied";
        });

        return builder;
    }

    /// <summary>
    /// Add the Countryside database
    /// </summary>
    internal static WebApplicationBuilder AddCountrysideDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(CSIDEOptions.ConnectionStringName);

        var databaseOptions = builder.Configuration
            .GetSection(CSIDEOptions.SectionName)
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>();

        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, x =>
            {
                x.MigrationsHistoryTable("__EFMigrationsHistory", databaseOptions?.Schema);
                x.UseNodaTime();
                x.UseNetTopologySuite();
                x.MapEnum<SurveyStatus>("survey_status", databaseOptions?.Schema);
                x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            })
            .UseSnakeCaseNamingConvention();

            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
        });

        return builder;
    }
}
