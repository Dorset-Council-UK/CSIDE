namespace CSIDE.Shared.Options;

public record CSIDEOptions
{
    public const string SectionName = "CSIDE";
    public const string ConnectionStringName = "CSIDE";

    public required MappingOptions Mapping { get; init; }
    public required IDPrefixOptions IDPrefixes { get; init; }
    public required ApplicationInsightsOptions ApplicationInsights { get; init; }
    public required AzureAdOptions AzureAd { get; init; }
    public required AzureAIOptions AzureAI { get; init; }
    public required AzureBlobStorageOptions AzureBlobStorage { get; init; }
    public required KeyVaultOptions KeyVault { get; init; }
    public required GovNotifySettings GovNotify { get; init; }
    public required ApiKeyAuthenticationOptions ApiKeyAuthentication { get; init; }
    public ThemeOptions Theme { get; init; } = new();
    public DatabaseOptions Database { get; init; } = new();

    public string AppName { get; init; } = "CSIDE";
    public string Version { get; init; } = "0.1 ALPHA";
    public string PathBase { get; init; } = "";
    public bool UseHttpsRedirection { get; init; }
    public string LandownerDepositsPublicRegisterURL { get; init; } = "";
    public string PublicMaintenanceJobURL { get; init; } = "";
    public string PublicUnsubscribeURL { get; init; } = "";
}