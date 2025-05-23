using AzureAISearchWithOpenAI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureAISearchWithOpenAI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;
        IAzureBlobService _BlobService;
        private readonly IAzureAISearchService _azureAIService;
        public List<string> blobFileNames { get; set; } = new List<string>();
        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IAzureAISearchService azureAIService)
        {
            _logger = logger;
            _configuration = configuration;
            _BlobService = new AzureBlobService(_configuration["Storage:ConnectionStr"], _configuration["Storage:Container"]);
            _azureAIService = azureAIService;
        }

        public void OnGet()
        {
            blobFileNames = _BlobService.GetBlobs().Result;
        }

        public IActionResult OnPost(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return RedirectToPage();
                }

                string uploadedFileName = _BlobService.UploadFile(file);
                if (!string.IsNullOrEmpty(uploadedFileName))
                {
                    bool isIndexerSuccess = _azureAIService.UpdateIndexer();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: ex.Message);
            }
            return RedirectToPage();

        }
        public IActionResult OnPostDelete(string blobName)
        {
            _BlobService.DeleteBlob(blobName);
            return RedirectToPage();
        }
    }
}
