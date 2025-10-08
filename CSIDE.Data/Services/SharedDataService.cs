using CSIDE.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CSIDE.Data.Services
{
    public class SharedDataService(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        IBlobStorageService blobStorageService,
        IMemoryCache memoryCache,
        ILogger<SharedDataService> logger) : ISharedDataService
    {
        public async Task<IReadOnlyCollection<Parish>> GetParishes(CancellationToken ct = default)
        {
            //TODO - Cache
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.Parishes
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToArrayAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<Parish>> GetParishesIntersecting(Geometry geometry, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.Parishes
                .AsNoTracking()
                .Where(p => p.Geom.Intersects(geometry))
                .OrderBy(p => p.Name)
                .ToArrayAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<string?> GetParishCodeByParishId(int ParishId, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.ParishCodes
                .AsNoTracking()
                .Where(p => p.ParishId == ParishId)
                .Select(p => p.Code)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<Parish?> GetBestFitParish(Geometry geometry, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.Parishes
                .AsNoTracking()
                .Where(p => p.Geom.Intersects(geometry))
                .OrderByDescending(p => p.Geom.Intersection(geometry).Length)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<ContactType>> GetContactTypeOptions(CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.ContactTypes
                .AsNoTracking()
                .OrderBy(ct => ct.Id)
                .ToArrayAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<Media?> UploadMedia(FileUploadRequest fileRequest, CancellationToken ct = default)
        {
            var trustedFileNameForFileStorage = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(fileRequest.FileName)}";
            var originalFileName = fileRequest.FileName;
            var blobUrl = await blobStorageService.UploadFileToBlobAsync(trustedFileNameForFileStorage, fileRequest.ContentType, fileRequest.FileStream);

            if (blobUrl != null)
            {
                var mediaItem = new Media
                {
                    URL = blobUrl,
                    Title = originalFileName
                };
                await using var context = await contextFactory.CreateDbContextAsync(ct);
                context.Media.Add(mediaItem);
                await context.SaveChangesAsync(ct);
                return mediaItem;
            }
            return null;
        }

        public async Task<Contact> UpdateContact(Contact contact, CancellationToken ct = default)
        {
            //clear the 'ContactType' navigation property, otherwise ef core uses that which hasn't changed
            contact.ContactType = null;

            //get the existing job to enable the smarter change tracker.
            //Without this, all properties are identified as tracked, since
            //the DbContext is different from when the entity was queried
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var existingContact = await context.Contacts.FindAsync([contact.Id], ct)
                ?? throw new Exception($"Contact being edited (ID: {contact.Id}) was not found prior to updating");

            context.Entry(existingContact).CurrentValues.SetValues(contact);

            await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return contact;
        }

        public async Task<bool> DeleteContact(int contactId, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var existingContact = await context.Contacts.FindAsync([contactId], cancellationToken: ct) ?? throw new Exception($"Contact being deleted (ID: {contactId}) was not found prior to deleting");
            context.Contacts.Remove(existingContact);
            var affected = await context.SaveChangesAsync(ct).ConfigureAwait(false);
            return affected > 0;
        }

        public async Task<bool> DeleteMediaItem(int mediaItemId, CancellationToken ct = default)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var mediaToDelete = await context.Media.Where(m => m.Id == mediaItemId).FirstOrDefaultAsync(ct);
            if (mediaToDelete is not null)
            {
                context.Media.Remove(mediaToDelete);
                var affected = await context.SaveChangesAsync(ct).ConfigureAwait(false);

                if (!await blobStorageService.DeleteFileFromBlobByURLAsync(mediaToDelete.URL))
                {
                    logger.LogWarning("An error occurred deleting media item with URL {Url} from blob storage", mediaToDelete.URL);
                }
                return affected > 0;
            }
            return false;
        }
        public async Task<PointGeometryResult?> TransformCoordinates(double x, double y, int fromCrs, int toCrs, CancellationToken ct = default)
        {
            var sql = @"SELECT ST_Transform(ST_SetSRID(ST_MakePoint({0}, {1}), {2}), {3}) AS ""Geom""";

            await using var context = await contextFactory.CreateDbContextAsync(ct);
            return await context.Database
                .SqlQueryRaw<PointGeometryResult>(sql, x, y, fromCrs, toCrs)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
        }

        public async Task<bool> RotateMediaImage(int mediaItemId, int rotationDegrees, CancellationToken ct = default)
        {
            try
            {
                await using var context = await contextFactory.CreateDbContextAsync(ct);
                var mediaItem = await context.Media.Where(m => m.Id == mediaItemId).FirstOrDefaultAsync(ct);
                
                if (mediaItem is null)
                {
                    logger.LogWarning("Media item with ID {MediaItemId} not found", mediaItemId);
                    return false;
                }

                // Download the image from blob storage
                var imageStream = await blobStorageService.DownloadFileFromBlobByURLAsync(mediaItem.URL);
                if (imageStream is null)
                {
                    logger.LogWarning("Failed to download image from blob storage for media item {MediaItemId}", mediaItemId);
                    return false;
                }

                // Load and rotate the image
                using var image = await Image.LoadAsync(imageStream, ct);
                imageStream.Dispose();

                // Rotate the image based on degrees
                image.Mutate(x => x.Rotate(rotationDegrees));

                // Save rotated image to a memory stream
                using var rotatedStream = new MemoryStream();
                await image.SaveAsJpegAsync(rotatedStream, ct);
                rotatedStream.Position = 0;

                // Upload the rotated image back to blob storage (this will overwrite the existing image)
                var uri = new Uri(mediaItem.URL);
                var fileName = uri.PathAndQuery.Replace($"/{blobStorageService.GetType().Name}/", "", StringComparison.OrdinalIgnoreCase);
                
                // Extract just the filename from the URL
                var segments = uri.Segments;
                fileName = segments[^1]; // Get the last segment (filename)

                await blobStorageService.UploadFileToBlobAsync(fileName, "image/jpeg", rotatedStream);

                logger.LogInformation("Successfully rotated media item {MediaItemId} by {RotationDegrees} degrees", mediaItemId, rotationDegrees);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred rotating media item {MediaItemId}", mediaItemId);
                return false;
            }
        }
    }
}
