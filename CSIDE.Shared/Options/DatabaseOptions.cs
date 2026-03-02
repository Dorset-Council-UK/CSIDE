namespace CSIDE.Shared.Options;

public record DatabaseOptions
{
    public const string SectionName = "Database";

    public string? Schema { get; init; }
}
