using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;

namespace CSIDE.Services
{

    /// <summary>
    /// Service to upload and delete files from an Azure Blob Storage container
    /// </summary>
    /// <remarks>Taken from https://www.c-sharpcorner.com/article/upload-and-delete-files-in-azure-blob-storage-using-blazor-apps-with-net-7/</remarks>
    public class BlobStorageService : IBlobStorageService
    {
        private IConfiguration _configuration;
        private ILogger<BlobStorageService> logger;
        string blobStorageConnection = string.Empty;
        private string blobContainerName = string.Empty;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            _configuration = configuration;
            blobStorageConnection = _configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            blobContainerName = _configuration.GetValue<string>("AzureBlobStorage:ContainerName");
            this.logger = logger;
        }

        public async Task<bool> DeleteFileFromBlobAsync(string fileName)
        {
            try
            {
                var container = new BlobContainerClient(blobStorageConnection, blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);
                var blob = container.GetBlobClient(fileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred deleting a file from blob storage");
                throw;
            }
        }

        public async Task<bool> DeleteFileFromBlobByURLAsync(string url)
        {
            Uri uri = new(url);
            string fileName = uri.PathAndQuery.Replace($"/{blobContainerName}/", "");
            return await DeleteFileFromBlobAsync(fileName);
        }

        public async Task<string> UploadFileToBlobAsync(string strFileName, string contentType, Stream fileStream)
        {
            try
            {
                var container = new BlobContainerClient(blobStorageConnection, blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(PublicAccessType.Blob);
                var blob = container.GetBlobClient("media/" + strFileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
                var urlString = blob.Uri.ToString();
                return urlString;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred uploading a media item to blob storage");
                throw;
            }
        }
    }
}
