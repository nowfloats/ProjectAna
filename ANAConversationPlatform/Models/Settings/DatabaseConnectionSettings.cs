using Newtonsoft.Json;

namespace ANAConversationPlatform.Models.Settings
{
    public class DatabaseConnectionSettings
    {
        [JsonProperty("ConnectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty("TemplateCollectionName")]
        public string TemplateCollectionName { get; set; }

        [JsonProperty("ContentCollectionName")]
        public string ContentCollectionName { get; set; }

        [JsonProperty("DatabaseName")]
        public string DatabaseName { get; set; }
    }
}