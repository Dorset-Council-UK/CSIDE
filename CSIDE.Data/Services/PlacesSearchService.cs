using CSIDE.Data.Models.Shared;
using CSIDE.Shared.Options;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CSIDE.Data.Services;

public class PlacesSearchService(IHttpClientFactory httpClientFactory, IOptions<MappingOptions> MappingOptions) : IPlacesSearchService
{
    private readonly string apiKey = MappingOptions.Value.OSMapsAPIKey;

    public async Task<List<SimpleAddress>> GetAddresses(string searchInput)
    {
        //figure out what we are searching for (UPRN, Postcode or Free Text)

        var searchType = GetAddressSearchTypeFromSearchInputString(searchInput);

        using var httpClient = httpClientFactory.CreateClient();

        var baseAddress = "https://api.os.uk/search/places/v1/";
        
        var url = baseAddress;
        switch (searchType)
        {
            case AddressSearchType.UPRN:
                url += $"uprn?uprn={searchInput}";
                break;
            case AddressSearchType.Postcode:
                url += $"postcode?postcode={searchInput}";
                break;
            case AddressSearchType.FreeText:
                url += $"find?query={searchInput}";
                break;
        }
        httpClient.DefaultRequestHeaders.Add("key", apiKey);
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var addresses = JsonSerializer.Deserialize<OSPlacesAPIResult>(responseString);
        if (addresses is not null && addresses.Results is not null)
        {
            return [.. addresses.Results.Select(x => new SimpleAddress(long.Parse(x.DPA!.UPRN!,CultureInfo.InvariantCulture), x.DPA.Address))];
        }
        return [];
    }

    public async Task<List<SimpleAddress>> GetAddressesByGeometry(string geojson)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var baseAddress = "https://api.os.uk/search/places/v1/polygon";
        var url = baseAddress;

        httpClient.DefaultRequestHeaders.Add("key", MappingOptions.Value.OSMapsAPIKey);

        var content = new StringContent(geojson, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var addresses = JsonSerializer.Deserialize<OSPlacesAPIResult>(responseString);
        if (addresses is not null && addresses.Results is not null)
        {
            return [.. addresses.Results.Select(x => new SimpleAddress(long.Parse(x.DPA!.UPRN!, CultureInfo.InvariantCulture), x.DPA.Address))];
        }
        return [];
    }

    private static AddressSearchType GetAddressSearchTypeFromSearchInputString(string searchInput)
    {
        if (long.TryParse(searchInput, CultureInfo.InvariantCulture, out _))
        {
            return AddressSearchType.UPRN;
        }

        if (Regex.IsMatch(searchInput, "^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$"))
        {
            return AddressSearchType.Postcode;
        }

        return AddressSearchType.FreeText;
    }

    public async Task<GazetteerEntry?> GetPlaceByName(string searchInput)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var baseAddress = "https://api.os.uk/search/names/v1/find";

        var url = baseAddress;
        string typeFilters = "&fq=LOCAL_TYPE:City LOCAL_TYPE:Hamlet LOCAL_TYPE:Other_Settlement LOCAL_TYPE:Town LOCAL_TYPE:Village";
        string bboxFilter = $"&fq=BBOX:{MappingOptions.Value.StartBounds.MinX},{MappingOptions.Value.StartBounds.MinY},{MappingOptions.Value.StartBounds.MaxX},{MappingOptions.Value.StartBounds.MaxY}";
        url += $"?query={searchInput}&maxResults=1{typeFilters}{bboxFilter}";

        httpClient.DefaultRequestHeaders.Add("key", apiKey);
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var places = JsonSerializer.Deserialize<OSNamesAPIResult>(responseString);
        if (places is not null && places.Results is not null)
        {
            return places.Results.FirstOrDefault()?.GazetteerEntry;
        }
        return null;
    }

}

enum AddressSearchType
{
    UPRN,
    Postcode,
    FreeText,
}
