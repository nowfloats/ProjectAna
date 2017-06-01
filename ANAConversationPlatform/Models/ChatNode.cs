using ANAConversationPlatform.Models.Sections;
using System.Collections.Generic;

namespace ANAConversationPlatform.Models
{
    public class ChatNode
    {
        public ChatNode() { }
        public ChatNode(string id)
        {
            this.Id = id;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public EmotionEnum Emotion { get; set; }
        public int TimeoutInMs { get; set; }
        public NodeTypeEnum NodeType { get; set; } = NodeTypeEnum.Combination;
        public List<Section> Sections { get; set; } = new List<Section>();
        public List<Button> Buttons { get; set; } = new List<Button>();
        public string VariableName { get; set; }
        public string ApiMethod { get; set; }
        public string ApiUrl { get; set; }
        public string NextNodeId { get; set; }
        public string[] RequiredVariables { get; set; }
        public string GroupName { get; set; }
    }

    public enum NodeTypeEnum
    {
        Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml, ApiCall, Combination
    };

    public enum EmotionEnum
    {
        Cool, Happy, Excited, Neutral, Sad, Irritated, Angry
    };
}