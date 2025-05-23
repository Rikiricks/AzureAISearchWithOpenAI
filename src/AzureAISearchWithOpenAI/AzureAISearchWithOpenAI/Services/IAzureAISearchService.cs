using Azure;
using Azure.AI.OpenAI;
using AzureAISearchWithOpenAI.Models;
using OpenAI.Chat;

namespace AzureAISearchWithOpenAI.Services
{
    public interface IAzureAISearchService
    {
        bool UpdateIndexer();
        AzureCustomResult SearchResultByAIService(string input);

        Task<ChatCompletion> SearchResultByOpenAI(string chatInput);
    }
}
