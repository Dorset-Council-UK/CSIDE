namespace CSIDE.Shared.Options;

public record StepUpAuthenticationOptions
{
    public const string SectionName = "Authentication";

    public bool EnableManagementStepUp { get; init; } = true;
    public bool UseStepUpExplainerPage { get; init; } = true;
}