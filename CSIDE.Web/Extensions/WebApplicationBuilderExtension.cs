using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using CSIDE.Data;
using CSIDE.Data.Models.Surveys;
using CSIDE.Shared.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AspNetCore.Builder;

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

        builder.Services
            .Configure<CSIDEOptions>(sectionCSIDE)
            .Configure<ApplicationInsightsOptions>(sectionApplicationInsights)
            .Configure<MappingOptions>(sectionMapping)
            .Configure<KeyVaultOptions>(sectionKeyVault)
            .Configure<ThemeOptions>(sectionTheme)
            .Configure<AzureBlobStorageOptions>(sectionAzureBlobStorage)
            .Configure<NetworkingOptions>(sectionNetworking)
            .Configure<IDPrefixOptions>(sectionIDPrefixes);

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
        const string b2cClientName = "B2CResilient";

        var azureAdSection = builder.Configuration
            .GetSection(CSIDEOptions.SectionName)
            .GetSection(AzureAdOptions.SectionName);

        // Register a named HttpClient for B2C with resilience
        builder.Services
            .AddHttpClient(b2cClientName)
            .AddStandardResilienceHandler();

        // Add microsoft identity web app authentication
        builder.Services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(azureAdSection);
        builder.Services.AddRazorPages();
        builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

        // Configure OpenIdConnectOptions to use our resilient HttpClient
        builder.Services
            .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<IHttpClientFactory>((options, httpClientFactory) =>
            {
                options.Backchannel = httpClientFactory.CreateClient(b2cClientName);
                options.SignedOutRedirectUri = "/account/signedout";
                options.AccessDeniedPath = "/account/accessdenied";
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

        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, x =>
            {
                x.MigrationsHistoryTable("__EFMigrationsHistory", "cside");
                x.UseNodaTime();
                x.UseNetTopologySuite();
                x.MapEnum<SurveyStatus>("survey_status");
            });
            options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

        }, lifetime: ServiceLifetime.Transient);

        return builder;
    }
}
