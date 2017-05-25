using Newtonsoft.Json;

namespace ANAConversationPlatform.Models.Settings
{
    public class DatabaseConnectionSettings
    {
        public string ConnectionString { get; set; }
        public string TemplateCollectionName { get; set; }
        public string ContentCollectionName { get; set; }
        public string DatabaseName { get; set; }
        public bool CacheContent { get; set; }
        public string ActivityEventLogCollectionName { get; set; }
    }
}