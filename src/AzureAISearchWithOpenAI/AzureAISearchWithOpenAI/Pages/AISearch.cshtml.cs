using AzureAISearchWithOpenAI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureAISearchWithOpenAI.Pages
{
    public class AISearchModel : PageModel
    {
        private readonly IAzureAISearchService _service;
        private readonly ILogger<AISearchModel> _searchLogger;

        public string Output { get; set; } = "";

        public AISearchModel(IAzureAISearchService service, ILogger<AISearchModel> searchLogger)
        {
            _service = service;
            this._searchLogger = searchLogger;
        }
        public void OnGet()
        {
            
        }

        public async void OnPost(string Searchinput)
        {
            try
            {
                // AI Search
                var response = _service.SearchResultByAIService(Searchinput);
                Output = response.content;

                // OpenAI Search
                //var response = await _service.SearchResultByOpenAI(Searchinput);
                //Output = response.Content.FirstOrDefault().Text;

            }
            catch (Exception ex)
            {
            }
        }
    }
}
