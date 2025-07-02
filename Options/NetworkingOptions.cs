namespace CSIDE.Options;

public record NetworkingOptions
{
    public const string SectionName = "Networking";

    public string[] KnownProxies { get; init; } = [];

}
