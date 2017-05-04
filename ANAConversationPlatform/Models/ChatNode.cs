using ANAConversationPlatform.Models.Sections;
using System.Collections.Generic;

namespace ANAConversationPlatform.Models
{
    public class ChatNode
    {
        public ChatNode(string id)
        {
            this.Id = id;
        }

        public string Name { get; set; } = null;
        public string HeaderText { get; set; } = null;
        public string Id { get; set; }
        public EmotionEnum Emotion { get; set; }
        public int TimeoutInMs { get; set; } = 15000;
        public NodeTypeEnum NodeType { get; set; } = NodeTypeEnum.Combination;
        public List<Section> Sections { get; set; } = new List<Section>();
        public List<Button> Buttons { get; set; } = new List<Button>();
        public DisplayTypeEnum DisplayType { get; set; } = DisplayTypeEnum.Inline;
        public string VariableName { get; set; } = null;
        public string ApiMethod { get; set; } = null;
        public string ApiUrl { get; set; } = null;
        public string NextNodeId { get; set; } = null;
        public string[] RequiredVariables { get; set; } = null;
    }

    public enum DisplayTypeEnum
    {
        Inline,
        FullscreenButtonList,
        FullscreenApiCall
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