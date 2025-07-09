
namespace CSIDE.Services
{
    internal interface IMapLinkHelperService
    {
        public string GenerateMapLink(string template, double x, double y, int? zoom);
        public string GenerateMapLink(string template, double[] bbox, char? bboxSeperator);
    }
}