namespace CSIDE.Shared.Options;

public record AzureAIOptions
{
    public const string SectionName = "AzureAI";

    public string LanguageEndpoint { get; init; } = "";
    public string LanguageApiKey { get; init; } = "";
    public string ContentSafetyEndpoint { get; init; } = "";
    public string ContentSafetyApiKey { get; init; } = "";
}
