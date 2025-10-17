using CSIDE.Data.Models.Shared;
using NetTopologySuite.Geometries;

namespace CSIDE.Data.Services
{
    public interface ISharedDataService
    {
        Task<IReadOnlyCollection<Parish>> GetParishes(CancellationToken ct = default);
        Task<IReadOnlyCollection<Parish>> GetParishesIntersecting(Geometry geometry, CancellationToken ct = default);
        Task<string?> GetParishCodeByParishId(int ParishId, CancellationToken ct = default);
        Task<Parish?> GetBestFitParish(Geometry geometry, CancellationToken ct = default);
        Task<IReadOnlyCollection<ContactType>> GetContactTypeOptions(CancellationToken ct = default);
        Task<Contact> UpdateContact(Contact contact, CancellationToken ct = default);
        Task<Media?> UploadMedia(FileUploadRequest fileRequest, CancellationToken ct = default);
        Task<bool> DeleteContact(int contactId, CancellationToken ct = default);
        Task<bool> DeleteMediaItem(int mediaItemId, CancellationToken ct = default);
        Task<PointGeometryResult?> TransformCoordinates(double x, double y, int fromCrs, int toCrs, CancellationToken ct = default);
        Task<bool> RotateMediaImage(int mediaItemId, int rotationDegrees, CancellationToken ct = default);
        Task<string> RedactPII(string text, CancellationToken ct = default);
        Task<bool> DoesTextContainProfanity(string text, CancellationToken ct = default);
    }
}