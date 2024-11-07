using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace CSIDE.Services
{

    /// <summary>
    /// Service to upload and delete files from an Azure Blob Storage container
    /// </summary>
    /// <remarks>Taken from https://www.c-sharpcorner.com/article/upload-and-delete-files-in-azure-blob-storage-using-blazor-apps-with-net-7/</remarks>
    public class BlobStorageService : IBlobStorageService
    {
        private IConfiguration _configuration;
        string blobStorageConnection = string.Empty;
        private string blobContainerName = string.Empty;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            blobStorageConnection = _configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            blobContainerName = _configuration.GetValue<string>("AzureBlobStorage:ContainerName");
        }

        public async Task<bool> DeleteFileToBlobAsync(string strFileName)
        {
            try
            {
                var container = new BlobContainerClient(blobStorageConnection, blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                var blob = container.GetBlobClient(strFileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                return true;
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<string> UploadFileToBlobAsync(string strFileName, string contentType, Stream fileStream)
        {
            try
            {
                var container = new BlobContainerClient(blobStorageConnection, blobContainerName);
                var createResponse = await container.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                var blob = container.GetBlobClient("media/" + strFileName);
                await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
                var urlString = blob.Uri.ToString();
                return urlString;
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex.ToString());
                throw;
            }
        }
    }
}
