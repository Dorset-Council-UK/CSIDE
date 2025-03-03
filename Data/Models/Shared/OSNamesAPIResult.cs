using System.Text.Json.Serialization;

namespace CSIDE.Data.Models.Shared
{
    public class NamesHeader
    {
        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("query")]
        public string? Query { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("totalresults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("format")]
        public string? Format { get; set; }

        [JsonPropertyName("maxresults")]
        public int MaxResults { get; set; }
    }

    public class GazetteerEntry
    {
        [JsonPropertyName("ID")]
        public string? Id { get; set; }

        [JsonPropertyName("NAMES_URI")]
        public string? NamesUri { get; set; }

        [JsonPropertyName("NAME1")]
        public string? Name1 { get; set; }

        [JsonPropertyName("TYPE")]
        public string? Type { get; set; }

        [JsonPropertyName("LOCAL_TYPE")]
        public string? LocalType { get; set; }

        [JsonPropertyName("GEOMETRY_X")]
        public decimal GeometryX { get; set; }

        [JsonPropertyName("GEOMETRY_Y")]
        public decimal GeometryY { get; set; }

        [JsonPropertyName("MOST_DETAIL_VIEW_RES")]
        public int MostDetailViewRes { get; set; }

        [JsonPropertyName("LEAST_DETAIL_VIEW_RES")]
        public int LeastDetailViewRes { get; set; }

        [JsonPropertyName("MBR_XMIN")]
        public decimal MbrXMin { get; set; }

        [JsonPropertyName("MBR_YMIN")]
        public decimal MbrYMin { get; set; }

        [JsonPropertyName("MBR_XMAX")]
        public decimal MbrXMax { get; set; }

        [JsonPropertyName("MBR_YMAX")]
        public decimal MbrYMax { get; set; }

        [JsonPropertyName("POSTCODE_DISTRICT")]
        public string? PostcodeDistrict { get; set; }

        [JsonPropertyName("POSTCODE_DISTRICT_URI")]
        public string? PostcodeDistrictUri { get; set; }

        [JsonPropertyName("POPULATED_PLACE")]
        public string? PopulatedPlace { get; set; }

        [JsonPropertyName("POPULATED_PLACE_URI")]
        public string? PopulatedPlaceUri { get; set; }

        [JsonPropertyName("POPULATED_PLACE_TYPE")]
        public string? PopulatedPlaceType { get; set; }

        [JsonPropertyName("COUNTY_UNITARY")]
        public string? CountyUnitary { get; set; }

        [JsonPropertyName("COUNTY_UNITARY_URI")]
        public string? CountyUnitaryUri { get; set; }

        [JsonPropertyName("COUNTY_UNITARY_TYPE")]
        public string? CountyUnitaryType { get; set; }

        [JsonPropertyName("REGION")]
        public string? Region { get; set; }

        [JsonPropertyName("REGION_URI")]
        public string? RegionUri { get; set; }

        [JsonPropertyName("COUNTRY")]
        public string? Country { get; set; }

        [JsonPropertyName("COUNTRY_URI")]
        public string? CountryUri { get; set; }
    }

    public class NamesResult
    {
        [JsonPropertyName("GAZETTEER_ENTRY")]
        public GazetteerEntry? GazetteerEntry { get; set; }
    }

    public class OSNamesAPIResult
    {
        [JsonPropertyName("header")]
        public NamesHeader? Header { get; set; }

        [JsonPropertyName("results")]
        public List<NamesResult>? Results { get; set; }
    }
}
