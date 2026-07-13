using CSIDE.Data.Models.Shared;
using CSIDE.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CSIDE.Data.Services;

public partial class PlacesSearchService(IHttpClientFactory httpClientFactory, IOptions<MappingOptions> MappingOptions, ILogger<PlacesSearchService> logger) : IPlacesSearchService
{
    private readonly string apiKey = MappingOptions.Value.OSMapsAPIKey;

    public async Task<List<SimpleAddress>> GetAddresses(string searchInput, CancellationToken ct = default)
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
        var response = await httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync(ct);
        var addresses = JsonSerializer.Deserialize<OSPlacesAPIResult>(responseString);
        if (addresses is not null && addresses.Results is not null)
        {
            return [.. addresses.Results.Select(x => new SimpleAddress(long.Parse(x.DPA!.UPRN!,CultureInfo.InvariantCulture), x.DPA.Address))];
        }
        return [];
    }

    public async Task<List<SimpleAddress>> GetAddressesByGeometry(string geojson, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var baseAddress = "https://api.os.uk/search/places/v1/polygon";
        var url = baseAddress;

        httpClient.DefaultRequestHeaders.Add("key", MappingOptions.Value.OSMapsAPIKey);

        var content = new StringContent(geojson, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, content, ct);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync(ct);
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

        if (PostcodeRegex().IsMatch(searchInput))
        {
            return AddressSearchType.Postcode;
        }

        return AddressSearchType.FreeText;
    }

    public async Task<GazetteerEntry?> GetPlaceByName(string searchInput, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateClient();

        var baseAddress = "https://api.os.uk/search/names/v1/find";

        var url = baseAddress;
        string typeFilterValue = "LOCAL_TYPE:City LOCAL_TYPE:Hamlet LOCAL_TYPE:Other_Settlement LOCAL_TYPE:Town LOCAL_TYPE:Village LOCAL_TYPE:Postcode";
        string typeFilters = $"&fq={Uri.EscapeDataString(typeFilterValue)}";
        string bboxFilterValue = $"BBOX:{MappingOptions.Value.StartBounds.MinX},{MappingOptions.Value.StartBounds.MinY},{MappingOptions.Value.StartBounds.MaxX},{MappingOptions.Value.StartBounds.MaxY}";
        string bboxFilter = $"&fq={Uri.EscapeDataString(bboxFilterValue)}";
        url += $"?query={Uri.EscapeDataString(searchInput)}&maxResults=1{typeFilters}{bboxFilter}";

        httpClient.DefaultRequestHeaders.Add("key", apiKey);
        var response = await httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync(ct);
        var places = JsonSerializer.Deserialize<OSNamesAPIResult>(responseString);
        if (places is not null && places.Results is not null)
        {
            return places.Results.FirstOrDefault()?.GazetteerEntry;
        }
        return null;
    }

    public async Task<GazetteerEntry?> GetNearestPlace(decimal x, decimal y, CancellationToken ct = default)
    {
        try
        {
            using var httpClient = httpClientFactory.CreateClient();

            var baseAddress = "https://api.os.uk/search/names/v1/nearest";

            var url = baseAddress;
            string typeFilterValue = "LOCAL_TYPE:City LOCAL_TYPE:Hamlet LOCAL_TYPE:Other_Settlement LOCAL_TYPE:Town LOCAL_TYPE:Village";
            string typeFilters = $"&fq={Uri.EscapeDataString(typeFilterValue)}";
            url += $"?point={decimal.Round(x,2)},{decimal.Round(y,2)}&radius=1000{typeFilters}";

            httpClient.DefaultRequestHeaders.Add("key", apiKey);
            var response = await httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(ct);
            var places = JsonSerializer.Deserialize<OSNamesAPIResult>(responseString);
            if (places is not null && places.Results is not null)
            {
                return places.Results.FirstOrDefault()?.GazetteerEntry;
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "An HTTP error occurred fetching the nearest place from the OS Names API");
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "The request timed out fetching the nearest place from the OS Names API");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize nearest place response from the OS Names API");
        }
        return null;
    }

    [GeneratedRegex("^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$")]
    private static partial Regex PostcodeRegex();
}



enum AddressSearchType
{
    UPRN,
    Postcode,
    FreeText,
}
