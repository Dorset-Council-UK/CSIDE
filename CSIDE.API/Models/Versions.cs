namespace CSIDE.API.Models;

internal static class Versions
{
    internal static readonly VersionInfo V1 = new(new(1, 0));
    internal static readonly IReadOnlyCollection<VersionInfo> All = [V1];
}
