namespace CSIDE.Shared.Options;

public record GovNotifyTemplates
{
    /// <summary>
    /// GovNotify template Id for informing team leads of a new bridge survey
    /// </summary>
    public required string NewBridgeSurvey { get; init; }

    /// <summary>
    /// GovNotify template Id for informing a user that a new maintenance job has been created
    /// </summary>
    public required string NewMaintenanceJobCreated { get; init; }

    /// <summary>
    /// GovNotify template Id for confirming a new maintenance subscription
    /// </summary>
    public required string NewMaintenanceSubscription { get; init; }
}
