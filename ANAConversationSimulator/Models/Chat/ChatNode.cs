using System.Collections.Generic;

namespace ANAConversationSimulator.Models.Chat
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
        public int TimeoutInMs { get; set; }
        public NodeTypeEnum NodeType { get; set; } = NodeTypeEnum.Combination;
        public List<Section> Sections { get; set; } = new List<Section>();
        public List<Button> Buttons { get; set; } = new List<Button>();
        public string VariableName { get; set; } = null;
        public string ApiMethod { get; set; } = null;
        public string ApiUrl { get; set; } = null;
        public string[] RequiredVariables { get; set; } = null;
        public string NextNodeId { get; set; } = null;
        public bool IsStartNode { get; set; }
        public string GroupName { get; set; }

        #region Card Node
        public string Header { get; set; }
        public string Footer { get; set; }
        public Placement Placement { get; set; }
        #endregion

        public bool PostToChat { get; set; }
    }

    public enum NodeTypeEnum
    {
        Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml, ApiCall, Combination, Card
    }

    public enum EmotionEnum
    {
        Cool, Happy, Excited, Neutral, Sad, Irritated, Angry
    }

    public enum Placement
    {
        Incomming, Outgoing, Center
    }
}