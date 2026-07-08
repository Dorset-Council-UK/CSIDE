using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Services
{
    public interface IPlacesSearchService
    {
        Task<List<SimpleAddress>> GetAddresses(string searchInput);
        Task<List<SimpleAddress>> GetAddressesByGeometry(string geojson);
        Task<GazetteerEntry?> GetPlaceByName(string searchInput);
    }
}