using Newtonsoft.Json.Linq;

namespace ANAConversationPlatform.Models.AgentChat
{
    public class SubmitChatHistoryModel
    {
        //[FromQuery]string CHAT_USER_ID, [FromQuery]string CHAT_USER_TOKEN, [FromQuery]string DEVICE_ID, [FromQuery] string AGENT, [FromBody]JArray sections
        public string CHAT_USER_ID { get; set; }
        public string CHAT_USER_TOKEN { get; set; }
        public string DEVICE_ID { get; set; }
        public string AGENT { get; set; }
        public JArray HISTORY { get; set; }
    }
}
