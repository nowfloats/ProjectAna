using System;
using System.Collections.Generic;

namespace ANAConversationPlatform.Models.Activity
{
    public class ChatActivityEvent
    {
        public string _id { get; set; }
        public string EventCategory { get; set; }
        public string EventChannel { get; set; }
        public string EventName { get; set; }
        public Dictionary<string, string> EventData { get; set; }
        public Dictionary<string, string> UserData { get; set; }
        public string UserId { get; set; }
        public string NodeId { get; set; }
        public DateTime EventDateTime { get; set; }
    }
}