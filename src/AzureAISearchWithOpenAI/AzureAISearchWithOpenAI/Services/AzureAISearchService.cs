using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using AzureAISearchWithOpenAI.Models;
using OpenAI;
using OpenAI.Chat;
using System.Text;

namespace AzureAISearchWithOpenAI.Services
{
    public class AzureAISearchService : IAzureAISearchService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureAISearchService> _logger;

        private readonly string _azureOpenAIApiBase = string.Empty;
        private readonly string _azureOpenAIKey = string.Empty;
        private readonly string _azuresearchServiceEndpoint = string.Empty;
        private readonly string _azuresearchIndexName = string.Empty;
        private readonly string _azuresearchApiKey = string.Empty;
        private readonly string _azureOpenAIDeploymentId = string.Empty;
        private readonly string _azurequeryKey = string.Empty;

        private readonly OpenAIClient _client;
        private ChatCompletionOptions _options;

        public AzureAISearchService(IConfiguration configuration, ILogger<AzureAISearchService> logger)
        {
            this._configuration = configuration;
            this._logger = logger;


            _azuresearchServiceEndpoint = _configuration.GetValue<string>("AISearchServiceEndpoint");
            _azuresearchIndexName = _configuration.GetValue<string>("AISearchIndexName");
            _azuresearchApiKey = _configuration.GetValue<string>("AISearchApiKey");
            _azurequeryKey = _configuration.GetValue<string>("QueryKey");

            _azureOpenAIApiBase = _configuration.GetValue<string>("AzOpenAIApiBase");
            _azureOpenAIKey = _configuration.GetValue<string>("AzOpenAIKey");
            _azureOpenAIDeploymentId = _configuration.GetValue<string>("AzOpenAIDeploymentId");

            _client = new AzureOpenAIClient(new Uri(_azureOpenAIApiBase), new AzureKeyCredential(_azureOpenAIKey));
        }
        public AzureCustomResult SearchResultByAIService(string input)
        {
            AzureCustomResult output = new AzureCustomResult();
            AzureKeyCredential cred = new AzureKeyCredential(_azurequeryKey);
            var client = new SearchIndexClient(new Uri(_azuresearchServiceEndpoint), cred);
            var searchClient = client.GetSearchClient(_azuresearchIndexName);

            var response = searchClient.Search<AzureCustomResult>(input).Value;
            StringBuilder sb = new StringBuilder();
            foreach (SearchResult<AzureCustomResult> result in response.GetResults())
            {
                output.content = result.Document.content;
                output.metadata_storage_path = result.Document.metadata_storage_path;

                output.people = result.Document.people;
                output.organizations = result.Document.organizations;
                output.locations = result.Document.locations;
            }

            return output;
        }

        public async Task<ChatCompletion> SearchResultByOpenAI(string chatInput)
        {

            ChatClient chatClient = _client.GetChatClient(_azureOpenAIDeploymentId);
            ChatCompletion completion = chatClient.CompleteChat(
                    [
          // System messages represent instructions or other guidance about how the assistant should behave
             new SystemChatMessage("You are a helpful assistant that talks like a pirate."),
          // User messages represent user input, whether historical or the most recent input
             new UserChatMessage("Hi, can you help me?"),
          // Assistant messages in a request represent conversation history for responses
             new AssistantChatMessage("Arrr! Of course, me hearty! What can I do for ye?"),
             new UserChatMessage("What's the best way to train a parrot?"),
            ]);
            return completion;
        }

        public bool UpdateIndexer()
        {
            try
            {
                string iName = _configuration["AISearchIndexerName"];

                SearchIndexerClient indexerClient = new SearchIndexerClient(
                                    new Uri(_configuration["AISearchServiceEndpoint"]),
                                    new AzureKeyCredential(_configuration["AISearchApiKey"]));

                Response response = indexerClient.RunIndexer(iName);
                if (response != null)
                {
                    if (response.Status == 202)
                    {
                        Thread.Sleep(5000);

                        if (IndexerStatus(indexerClient, iName))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, message: ex.InnerException?.Message);
                throw;
            }
        }

        private bool IndexerStatus(SearchIndexerClient client, string iName)
        {
            bool isIndexerUpdated = false;
            SearchIndexerStatus execInfo = client.GetIndexerStatus(iName);
            IndexerExecutionResult result = execInfo.LastResult;

            if (result.ErrorMessage != null)
            {
                _logger.LogError($"Error occured while updating Indexer. Error = {result.ErrorMessage}");
            }
            else
            {
                _logger.LogInformation("Indexer updated Success");
                isIndexerUpdated = true;
            }
            return isIndexerUpdated;
        }
    }
}
