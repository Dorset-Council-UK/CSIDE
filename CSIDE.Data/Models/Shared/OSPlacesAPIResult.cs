using System.Text.Json.Serialization;

namespace CSIDE.Data.Models.Shared
{
    public class DPA
    {
        [JsonPropertyName("UPRN")]
        public string? UPRN { get; set; }

        [JsonPropertyName("UDPRN")]
        public string? UDPRN { get; set; }

        [JsonPropertyName("ADDRESS")]
        public string? Address { get; set; }

        [JsonPropertyName("ORGANISATION_NAME")]
        public string? OrganisationName { get; set; }

        [JsonPropertyName("DEPARTMENT_NAME")]
        public string? DepartmentName { get; set; }

        [JsonPropertyName("SUB_BUILDING_NAME")]
        public string? SubBuildingName { get; set; }

        [JsonPropertyName("BUILDING_NAME")]
        public string? BuildingName { get; set; }

        [JsonPropertyName("BUILDING_NUMBER")]
        public string? BuildingNumber { get; set; }

        [JsonPropertyName("DEPENDENT_THOROUGHFARE_NAME")]
        public string? DependentThoroughfareName { get; set; }

        [JsonPropertyName("THOROUGHFARE_NAME")]
        public string? ThoroughfareName { get; set; }

        [JsonPropertyName("DOUBLE_DEPENDENT_LOCALITY")]
        public string? DoubleDependentLocality { get; set; }

        [JsonPropertyName("DEPENDENT_LOCALITY")]
        public string? DependentLocality { get; set; }

        [JsonPropertyName("POST_TOWN")]
        public string? PostTown { get; set; }

        [JsonPropertyName("POSTCODE")]
        public string? Postcode { get; set; }

        [JsonPropertyName("RPC")]
        public string? RPC { get; set; }

        [JsonPropertyName("X_COORDINATE")]
        public decimal XCoordinate { get; set; }

        [JsonPropertyName("Y_COORDINATE")]
        public decimal YCoordinate { get; set; }

        [JsonPropertyName("LNG")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("LAT")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("STATUS")]
        public string? Status { get; set; }

        [JsonPropertyName("LOGICAL_STATUS_CODE")]
        public string? LogicalStatusCode { get; set; }

        [JsonPropertyName("CLASSIFICATION_CODE")]
        public string? ClassificationCode { get; set; }

        [JsonPropertyName("CLASSIFICATION_CODE_DESCRIPTION")]
        public string? ClassificationCodeDescription { get; set; }

        [JsonPropertyName("LOCAL_CUSTODIAN_CODE")]
        public int LocalCustodianCode { get; set; }

        [JsonPropertyName("LOCAL_CUSTODIAN_CODE_DESCRIPTION")]
        public string? LocalCustodianCodeDescription { get; set; }

        [JsonPropertyName("COUNTRY_CODE")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("COUNTRY_CODE_DESCRIPTION")]
        public string? CountryCodeDescription { get; set; }

        [JsonPropertyName("POSTAL_ADDRESS_CODE")]
        public string? PostalAddressCode { get; set; }

        [JsonPropertyName("POSTAL_ADDRESS_CODE_DESCRIPTION")]
        public string? PostalAddressCodeDescription { get; set; }

        [JsonPropertyName("BLPU_STATE_CODE")]
        public string? BLPUStateCode { get; set; }

        [JsonPropertyName("BLPU_STATE_CODE_DESCRIPTION")]
        public string? BLPUStateCodeDescription { get; set; }

        [JsonPropertyName("TOPOGRAPHY_LAYER_TOID")]
        public string? TopographyLayerToid { get; set; }

        [JsonPropertyName("WARD_CODE")]
        public string? WardCode { get; set; }

        [JsonPropertyName("PARISH_CODE")]
        public string? ParishCode { get; set; }

        [JsonPropertyName("PARENT_UPRN")]
        public string? ParentUPRN { get; set; }

        [JsonPropertyName("LAST_UPDATE_DATE")]
        public string? LastUpdateDate { get; set; }

        [JsonPropertyName("ENTRY_DATE")]
        public string? EntryDate { get; set; }

        [JsonPropertyName("LEGAL_NAME")]
        public string? LegalName { get; set; }

        [JsonPropertyName("BLPU_STATE_DATE")]
        public string? BLPUStateDate { get; set; }

        [JsonPropertyName("LANGUAGE")]
        public string? Language { get; set; }

        [JsonPropertyName("MATCH")]
        public decimal Match { get; set; }

        [JsonPropertyName("MATCH_DESCRIPTION")]
        public string? MatchDescription { get; set; }

        [JsonPropertyName("DELIVERY_POINT_SUFFIX")]
        public string? DeliveryPointSuffix { get; set; }
    }

    public class PlacesHeader
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

        [JsonPropertyName("dataset")]
        public string? Dataset { get; set; }

        [JsonPropertyName("lr")]
        public string? Lr { get; set; }

        [JsonPropertyName("maxresults")]
        public int MaxResults { get; set; }

        [JsonPropertyName("matchprecision")]
        public decimal MatchPrecision { get; set; }

        [JsonPropertyName("epoch")]
        public string? Epoch { get; set; }

        [JsonPropertyName("lastupdate")]
        public string? LastUpdate { get; set; }

        [JsonPropertyName("output_srs")]
        public string? OutputSrs { get; set; }
    }

    public class LPI
    {
        [JsonPropertyName("UPRN")]
        public string? UPRN { get; set; }

        [JsonPropertyName("ADDRESS")]
        public string? Address { get; set; }

        [JsonPropertyName("USRN")]
        public string? USRN { get; set; }

        [JsonPropertyName("LPI_KEY")]
        public string? LPIKey { get; set; }

        [JsonPropertyName("ORGANISATION")]
        public string? Organisation { get; set; }

        [JsonPropertyName("SAO_START_NUMBER")]
        public int SAOStartNumber { get; set; }

        [JsonPropertyName("SAO_START_SUFFIX")]
        public string? SAOStartSuffix { get; set; }

        [JsonPropertyName("SAO_END_NUMBER")]
        public int SAOEndNumber { get; set; }

        [JsonPropertyName("SAO_END_SUFFIX")]
        public string? SAOEndSuffix { get; set; }

        [JsonPropertyName("SAO_TEXT")]
        public string? SAOText { get; set; }

        [JsonPropertyName("PAO_START_NUMBER")]
        public int PAOStartNumber { get; set; }

        [JsonPropertyName("PAO_START_SUFFIX")]
        public string? PAOStartSuffix { get; set; }

        [JsonPropertyName("PAO_END_NUMBER")]
        public int PAOEndNumber { get; set; }

        [JsonPropertyName("PAO_END_SUFFIX")]
        public string? PAOEndSuffix { get; set; }

        [JsonPropertyName("PAO_TEXT")]
        public string? PAOText { get; set; }

        [JsonPropertyName("STREET_DESCRIPTION")]
        public string? StreetDescription { get; set; }

        [JsonPropertyName("LOCALITY_NAME")]
        public string? LocalityName { get; set; }

        [JsonPropertyName("TOWN_NAME")]
        public string? TownName { get; set; }

        [JsonPropertyName("ADMINISTRATIVE_AREA")]
        public string? AdministrativeArea { get; set; }

        [JsonPropertyName("AREA_NAME")]
        public string? AreaName { get; set; }

        [JsonPropertyName("POSTCODE_LOCATOR")]
        public string? PostcodeLocator { get; set; }

        [JsonPropertyName("RPC")]
        public string? RPC { get; set; }

        [JsonPropertyName("X_COORDINATE")]
        public decimal XCoordinate { get; set; }

        [JsonPropertyName("Y_COORDINATE")]
        public decimal YCoordinate { get; set; }

        [JsonPropertyName("LNG")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("LAT")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("STATUS")]
        public string? Status { get; set; }

        [JsonPropertyName("LOGICAL_STATUS_CODE")]
        public string? LogicalStatusCode { get; set; }

        [JsonPropertyName("CLASSIFICATION_CODE")]
        public string? ClassificationCode { get; set; }

        [JsonPropertyName("CLASSIFICATION_CODE_DESCRIPTION")]
        public string? ClassificationCodeDescription { get; set; }

        [JsonPropertyName("LOCAL_CUSTODIAN_CODE")]
        public int LocalCustodianCode { get; set; }

        [JsonPropertyName("LOCAL_CUSTODIAN_CODE_DESCRIPTION")]
        public string? LocalCustodianCodeDescription { get; set; }

        [JsonPropertyName("COUNTRY_CODE")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("COUNTRY_CODE_DESCRIPTION")]
        public string? CountryCodeDescription { get; set; }

        [JsonPropertyName("POSTAL_ADDRESS_CODE")]
        public string? PostalAddressCode { get; set; }

        [JsonPropertyName("POSTAL_ADDRESS_CODE_DESCRIPTION")]
        public string? PostalAddressCodeDescription { get; set; }

        [JsonPropertyName("BLPU_STATE_CODE")]
        public string? BLPUStateCode { get; set; }

        [JsonPropertyName("BLPU_STATE_CODE_DESCRIPTION")]
        public string? BLPUStateCodeDescription { get; set; }

        [JsonPropertyName("TOPOGRAPHY_LAYER_TOID")]
        public string? TopographyLayerToid { get; set; }

        [JsonPropertyName("WARD_CODE")]
        public string? WardCode { get; set; }

        [JsonPropertyName("PARISH_CODE")]
        public string? ParishCode { get; set; }

        [JsonPropertyName("PARENT_UPRN")]
        public int ParentUPRN { get; set; }

        [JsonPropertyName("LAST_UPDATE_DATE")]
        public string? LastUpdateDate { get; set; }

        [JsonPropertyName("ENTRY_DATE")]
        public string? EntryDate { get; set; }

        [JsonPropertyName("LEGAL_NAME")]
        public string? LegalName { get; set; }

        [JsonPropertyName("BLPU_STATE_DATE")]
        public string? BLPUStateDate { get; set; }

        [JsonPropertyName("STREET_STATE_CODE")]
        public int StreetStateCode { get; set; }

        [JsonPropertyName("STREET_STATE_CODE_DESCRIPTION")]
        public string? StreetStateCodeDescription { get; set; }

        [JsonPropertyName("STREET_CLASSIFICATION_CODE")]
        public string? StreetClassificationCode { get; set; }

        [JsonPropertyName("STREET_CLASSIFICATION_CODE_DESCRIPTION")]
        public string? StreetClassificationCodeDescription { get; set; }

        [JsonPropertyName("LPI_LOGICAL_STATUS_CODE")]
        public int LPILogicalStatusCode { get; set; }

        [JsonPropertyName("LPI_LOGICAL_STATUS_CODE_DESCRIPTION")]
        public string? LPILogicalStatusCodeDescription { get; set; }

        [JsonPropertyName("LANGUAGE")]
        public string? Language { get; set; }

        [JsonPropertyName("MATCH")]
        public decimal Match { get; set; }

        [JsonPropertyName("MATCH_DESCRIPTION")]
        public string? MatchDescription { get; set; }
    }

    public class PlacesResult
    {
        [JsonPropertyName("DPA")]
        public DPA? DPA { get; set; }

        [JsonPropertyName("LPI")]
        public LPI? LPI { get; set; }
    }

    public class OSPlacesAPIResult
    {
        [JsonPropertyName("header")]
        public PlacesHeader? Header { get; set; }
        [JsonPropertyName("results")]
        public List<PlacesResult>? Results { get; set; }
    }
}
