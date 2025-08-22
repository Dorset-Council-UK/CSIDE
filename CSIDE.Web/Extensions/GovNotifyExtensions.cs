using CSIDE.Shared.Options;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Interfaces;

namespace Microsoft.AspNetCore.Builder;

internal static class GovNotifyExtensions
{
    /// <summary>
    /// Setup the GovNotify's Settings, HttpClient, HttpClientWrapper, and NotificationClient
    /// </summary>
    public static IServiceCollection AddGovNotify(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddGovNotifySettings(configuration)
            .AddGovNotifyHttpClient()
            .AddGovNotifyHttpClientWrapper()
            .AddGovNotifyNotificationClient();

        return services;
    }

    /// <summary>
    /// Setup the GovNotifySettings using the configuration section
    /// </summary>
    private static IServiceCollection AddGovNotifySettings(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("CSIDE").GetSection(GovNotifySettings.SectionName);
        services.Configure<GovNotifySettings>(section);

        return services;
    }

    /// <summary>
    /// Setup the GovNotify HttpClient using the HttpClientFactory
    /// </summary>
    /// <remarks>Uses Microsoft's new Http resilience library <see href="https://www.nuget.org/packages/Microsoft.Extensions.Http.Resilience/">Microsoft.Extensions.Http.Resilience</see></remarks>
    private static IServiceCollection AddGovNotifyHttpClient(this IServiceCollection services)
    {
        services
            .AddHttpClient("GovNotifyClient")
            .AddStandardResilienceHandler();

        return services;
    }

    /// <summary>
    /// Setup the GovNotify HttpClientWrapper using the HttpClientFactory
    /// </summary>
    private static IServiceCollection AddGovNotifyHttpClientWrapper(this IServiceCollection services)
    {
        services.AddTransient<IHttpClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("GovNotifyClient");
            return new HttpClientWrapper(httpClient);
        });

        return services;
    }

    /// <summary>
    /// Setup the GovNotify NotificationClient using the configured HttpClientWrapper
    /// </summary>
    private static IServiceCollection AddGovNotifyNotificationClient(this IServiceCollection services)
    {
        services.AddTransient<NotificationClient>(serviceProvider =>
        {
            var httpClientWrapper = serviceProvider.GetRequiredService<IHttpClient>();
            var options = serviceProvider.GetRequiredService<IOptions<GovNotifySettings>>();
            return new NotificationClient(httpClientWrapper, options.Value.ApiKey);
        });

        return services;
    }
}
