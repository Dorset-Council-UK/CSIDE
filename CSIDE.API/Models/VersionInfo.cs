using Asp.Versioning;
using System.Globalization;

namespace CSIDE.API.Models;

internal record VersionInfo(ApiVersion Version)
{
    internal string DocumentName => string.Format(CultureInfo.CurrentCulture, "v{0}", Version.MajorVersion);
}
