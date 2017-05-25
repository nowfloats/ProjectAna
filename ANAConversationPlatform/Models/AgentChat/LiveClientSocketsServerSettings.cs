using Newtonsoft.Json;

namespace ANAConversationPlatform.Models.AgentChat
{
    public class LiveClientSocketsServerSettings
    {
        [JsonProperty("Server")]
        public string Server { get; set; }

        [JsonProperty("AuthBasic")]
        public string AuthBasic { get; set; }
    }
}
