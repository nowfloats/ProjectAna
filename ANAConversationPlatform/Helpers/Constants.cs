using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace ANAConversationPlatform.Helpers
{
    public static class Constants
    {
        public const string CHAT_MEDIA_FOLDER_NAME = "ChatMedia";
        public enum LoggerEventId
        {
            ROCKET_CHAT_SDK_INIT_ERROR = 0,
            AGENT_CHAT_SUBMIT_HISTORY_ERR = 1,
            AGENT_CHAT_CREATE_USER_ERR = 2,
            AGENT_CHAT_CALLBACK_RECEIVED_ERR = 3,
            AGENT_CHAT_FIND_AGENT_ERR = 4,
            AGENT_CHAT_CREATE_USER = 5,
            MONGO_HELPER_ERROR = 6,
            CHAT_ACTION_ERROR = 7,
            HYBRID_CHAT_ACTION_ERROR = 8,
        }

        public static readonly JsonSerializerSettings PublishJsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };
    }
}
