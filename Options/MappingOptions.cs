namespace CSIDE.Options;

public record MappingOptions
{
    public const string SectionName = "Mapping";

    public required string OSMapsAPIKey { get; init; }
    public required string OSLicenceNumber { get; init; }
    public required string MapServerRoot { get; init; }
    public required string RouteLayer { get; init; }
    public required string InfrastructureLayer { get; init; }
    public StartBoundsOptions StartBounds { get; init; } = new();
    public string ExternalMappingSystemName { get; init; } = "";
    public string ExternalMappingSystemURL { get; init; } = "";
    public string MaintJobMapLinkTemplateURL { get; init; } = "";
    public string InfraItemMapLinkTemplateURL { get; init; } = "";
    public string DMMOMapLinkTemplateURL { get; init; } = "";
    public string LandownerDepositMapLinkTemplateURL { get; init; } = "";
    public string PPOMapLinkTemplateURL { get; init; } = "";
    public string RouteMapLinkTemplateURL { get; init; } = "";
    public string ExternalMappingSystemBridgeSurveyMapURL { get; init; } = "";
}