namespace CSIDE.Options;

internal record ApplicationInsightsOptions
{
    public const string SectionName = "ApplicationInsights";

    public string ConnectionString { get; init; } = "";
}
