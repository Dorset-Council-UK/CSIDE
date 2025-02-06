namespace CSIDE.Options;

internal record KeyVaultOptions
{
    public const string SectionName = "KeyVault";

    public string Name { get; init; } = "";
    public KeyVaultAzureAdOptions AzureAd { get; init; } = new();
}
