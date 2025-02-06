namespace CSIDE.Options;

internal record MappingOptions
{
    public const string SectionName = "Mapping";

    public required string OSMapsAPIKey { get; init; }
    public required string OSLicenceNumber { get; init; }
    public required string MapServerRoot { get; init; }
    public required string RouteLayer { get; init; }
    public required string InfrastructureLayer { get; init; }
    public StartBoundsOptions StartBounds { get; init; } = new();
}