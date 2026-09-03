using CSIDE.Data.Models.Shared;

namespace CSIDE.Data.Services
{
    public interface IPlacesSearchService
    {
        Task<List<SimpleAddress>> GetAddresses(string searchInput, CancellationToken ct = default);
        Task<List<SimpleAddress>> GetAddressesByGeometry(string geojson, CancellationToken ct = default);
        Task<GazetteerEntry?> GetPlaceByName(string searchInput, CancellationToken ct = default);
        Task<GazetteerEntry?> GetNearestPlace(decimal x, decimal y, CancellationToken ct = default);
    }
}