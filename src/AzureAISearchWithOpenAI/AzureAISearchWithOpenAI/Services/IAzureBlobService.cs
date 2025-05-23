namespace AzureAISearchWithOpenAI.Services
{
    public interface IAzureBlobService
    {
        Task<List<string>> GetBlobs();
        string UploadFile(IFormFile formFile);
        bool DeleteBlob(string blobName);
    }
}
