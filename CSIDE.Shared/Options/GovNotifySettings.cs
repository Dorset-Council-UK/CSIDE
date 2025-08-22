namespace CSIDE.Shared.Options;

public record GovNotifySettings
{
    public const string SectionName = "GovNotify";

    public required string ApiKey { get; init; }
    public required GovNotifyTemplates Templates { get; init; }
}
