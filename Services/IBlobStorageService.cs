namespace CSIDE.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileToBlobAsync(string strFileName, string contecntType, Stream fileStream);
        Task<bool> DeleteFileFromBlobAsync(string strFileName);
        Task<bool> DeleteFileFromBlobByURLAsync(string url);
    }
}
