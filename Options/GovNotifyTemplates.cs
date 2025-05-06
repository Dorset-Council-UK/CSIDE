namespace CSIDE.Options;

public record GovNotifyTemplates
{
    /// <summary>
    /// GovNotify template Id for informing team leads of a new bridge survey
    /// </summary>
    public required string NewBridgeSurvey { get; init; }
}
