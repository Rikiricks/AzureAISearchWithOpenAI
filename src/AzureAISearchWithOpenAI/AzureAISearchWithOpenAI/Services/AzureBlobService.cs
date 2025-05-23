using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace AzureAISearchWithOpenAI.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private BlobServiceClient _blobClient;
        private BlobContainerClient _containerClient;

        public AzureBlobService(string connectionStr, string containerName)
        {
            _blobClient = new BlobServiceClient(connectionStr);
            _containerClient = _blobClient.GetBlobContainerClient(containerName);
        }

        public async Task<List<string>> GetBlobs()
        {
            List<string> lstFileNames = new List<string>();

            try
            {                
                var blobs = _containerClient.GetBlobsAsync();
                await foreach (var blob in blobs)
                {
                    lstFileNames.Add(blob.Name);
                }
            }
            catch (Exception ex) { }
            return lstFileNames;
        }

        public string UploadFile(IFormFile formFile)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(formFile.FileName);

            using (Stream stream = formFile.OpenReadStream())
            {
                blobClient.Upload(stream, true);
            }

            return formFile.FileName;
        }

        public bool DeleteBlob(string blobName)
        {
            try
            {                               
                _containerClient.DeleteBlobIfExistsAsync(blobName);
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}
