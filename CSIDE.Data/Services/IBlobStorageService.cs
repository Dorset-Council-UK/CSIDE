namespace CSIDE.Data.Services;

public interface IBlobStorageService
{
    Task<string> UploadFileToBlobAsync(string fileName, string contentType, Stream fileStream);
    Task<bool> DeleteFileFromBlobAsync(string fileName);
    Task<bool> DeleteFileFromBlobByURLAsync(string url);
    Task<Stream?> DownloadFileFromBlobByURLAsync(string url);
}
