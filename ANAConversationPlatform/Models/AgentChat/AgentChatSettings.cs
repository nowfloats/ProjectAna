using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Models.AgentChat
{
    public class AgentChatSettings
    {
        public string DefaultAgentUserName { get; set; }
        public string APIBase { get; set; }
        public string APIUserName { get; set; }
        public string APIPassword { get; set; }
    }
}
