namespace CSIDE.Options;

public record ApplicationInsightsOptions
{
    public const string SectionName = "ApplicationInsights";

    public string ConnectionString { get; init; } = "";
}
