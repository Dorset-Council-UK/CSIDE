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

    /// <summary>
    /// GovNotify template Id for notifying about an update to the status of a maintenance job
    /// </summary>
    public required string MaintenanceJobUpdated { get; init; }

    /// <summary>
    /// GovNotify template Id for notifying a maintenance job being marked as a duplicate
    /// </summary>
    public required string MaintenanceJobDuplicate { get; init; }

    /// <summary>
    /// GovNotify template Id for notifying about a maintenance job being completed
    /// </summary>
    public required string MaintenanceJobCompleted { get; init; }

    /// <summary>
    /// GovNotify template Id for notifying about a new public comment being added to a maintenance job
    /// </summary>
    public required string MaintenanceCommentAdded { get; init; }
}
