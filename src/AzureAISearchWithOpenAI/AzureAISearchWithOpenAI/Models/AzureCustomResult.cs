namespace AzureAISearchWithOpenAI.Models
{
    public class AzureCustomResult
    {
        public string content { get; set; }
        public string metadata_storage_path { get; set; }

        public List<string> people { get; set; }

        public List<string> organizations { get; set; }

        public List<string> locations { get; set; }
        public string language { get; set; }
    }
}
