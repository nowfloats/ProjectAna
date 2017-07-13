using System.Collections.Generic;

namespace ANAConversationStudio.Models.Chat
{
    public class ChatFlowPack : BaseIdTimeStampEntity
    {
        public string ProjectId { get; set; }

        public List<ChatNode> ChatNodes { get; set; }
        public List<BaseContent> ChatContent { get; set; }
        public Dictionary<string, LayoutPoint> NodeLocations { get; set; }
    }
}
