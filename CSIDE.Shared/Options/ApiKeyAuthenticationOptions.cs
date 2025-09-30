namespace CSIDE.Shared.Options;

public record ApiKeyAuthenticationOptions
{
    public const string SectionName = "ApiKeyAuthentication";
    
    /// <summary>
    /// The name of the header that contains the API key.
    /// </summary>
    public string HeaderName { get; init; } = "X-API-Key";
    
    /// <summary>
    /// List of valid API keys for authentication.
    /// </summary>
    public List<string> ValidApiKeys { get; init; } = [];
    
}