namespace CSIDE.Shared.Options;

public record IDPrefixOptions
{
    public const string SectionName = "IDPrefixes";

    public string Maintenance { get; init; } = "";
    public string Infrastructure { get; init; } = "";
    public string DMMO { get; init; } = "";
    public string LandownerDeposit { get; init; } = "";
    public string PPO { get; init; } = "";
}