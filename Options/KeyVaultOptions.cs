namespace CSIDE.Options;

public record KeyVaultOptions
{
    public const string SectionName = "KeyVault";

    public string Name { get; init; } = "";
    public KeyVaultAzureAdOptions AzureAd { get; init; } = new();
}
