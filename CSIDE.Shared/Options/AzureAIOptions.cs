namespace CSIDE.Shared.Options;

public record AzureAIOptions
{
    public const string SectionName = "AzureAI";

    public string LanguageEndpoint { get; init; } = "";
    public string LanguageApiKey { get; init; } = "";
}
