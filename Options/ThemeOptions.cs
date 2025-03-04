namespace CSIDE.Options;

public record ThemeOptions
{
    public const string SectionName = "Theme";

    public string PrimaryColour { get; init; } = "#006754";
    public string IconsRoot { get; init; } = "https://gistaticprod.blob.core.windows.net/cside/icons";
}
