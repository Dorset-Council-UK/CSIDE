namespace CSIDE.Services;

internal class MapLinkHelperService() : IMapLinkHelperService
{
    public string GenerateMapLink(string template, double x, double y, int? zoom)
    {
        return GenerateMapLink(template, x, y, zoom, bbox: null, bboxSeperator: null);
    }
    public string GenerateMapLink(string template, double[] bbox, char? bboxSeperator)
    {
        return GenerateMapLink(template, x: null, y: null, zoom: null, bbox, bboxSeperator);
    }
    private static string GenerateMapLink(string template, double? x, double? y, int? zoom, double[]? bbox, char? bboxSeperator)
    {
        IFormatProvider fmt = System.Globalization.CultureInfo.InvariantCulture;
        if(bbox is not null && bbox.Length == 4)
        {
            //use bbox replacements
            template = template
                .Replace("{bbox}", string.Join(bboxSeperator ?? ',', bbox.Select(b => b.ToString(fmt))), StringComparison.Ordinal);
        }
        else
        {
            //use x/y/z replacements
            template = template
                .Replace("{x}", x?.ToString(fmt) ?? "", StringComparison.Ordinal)
                .Replace("{y}", y?.ToString(fmt) ?? "", StringComparison.Ordinal)
                .Replace("{z}", zoom?.ToString(fmt) ?? "", StringComparison.Ordinal);
        }
            
        return template;
    }

}
