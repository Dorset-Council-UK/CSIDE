namespace CSIDE.Options;

public record StartBoundsOptions
{
    public const string SectionName = "StartBounds";

    public double MinX { get; init; } = 330000;
    public double MinY { get; init; } = 67000;
    public double MaxX { get; init; } = 423000;
    public double MaxY { get; init; } = 133000;
};