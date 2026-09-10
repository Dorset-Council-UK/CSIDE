using Microsoft.Extensions.Localization;

namespace CSIDE.Web.Extensions;

public static class NullableBooleanExtensions
{
    public static string ToLocalizedDisplayString(
        this bool? value,
        IStringLocalizer localizer,
        string? nullLabel = null)
    {
        ArgumentNullException.ThrowIfNull(localizer);

        return value switch
        {
            true => localizer["Yes Label"].Value,
            false => localizer["No Label"].Value,
            null => nullLabel ?? localizer["Not Provided Label"].Value
        };
    }
}