namespace ANAConversationPlatform.Models.Settings
{
    public class DatabaseConnectionSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public string ProjectsCollectionName { get; set; }
        public string ChatFlowPacksCollectionName { get; set; }
        public string ActivityEventLogCollectionName { get; set; }
    }
}