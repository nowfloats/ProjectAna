using ANAConversationPlatform.Models.Sections;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ANAConversationPlatform.Models
{
	public class ChatNodeBase
	{
		public ChatNodeBase() { }
		public ChatNodeBase(string id) { this.Id = id; }
		public string Name { get; set; }
		public string Id { get; set; }

		//[JsonIgnore] //JsonIgnore should not be here //bsonIgnore should NOT be used here
		public bool IsStartNode { get; set; }

		public EmotionEnum Emotion { get; set; }
		public int TimeoutInMs { get; set; }
		public NodeTypeEnum NodeType { get; set; } = NodeTypeEnum.Combination;
		public List<Button> Buttons { get; set; } = new List<Button>();
		public string VariableName { get; set; }
		public string ApiMethod { get; set; }
		public string ApiUrl { get; set; }
		public string ApiResponseDataRoot { get; set; }
		public string NextNodeId { get; set; }
		public string[] RequiredVariables { get; set; }
		public string GroupName { get; set; }
		public string FlowId { get; set; }
		public SpecialChatNode SpecialNode { get; set; }

		#region Card Node
		public string CardHeader { get; set; }
		public string CardFooter { get; set; }
		public Placement? Placement { get; set; }
		#endregion
	}
	public class ChatNode : ChatNodeBase
	{
		public ChatNode() { }
		public ChatNode(string id) : base(id) { }
		public List<Section> Sections { get; set; } = new List<Section>();
	}
	public class ChatNodeRequest : ChatNodeBase
	{
		public List<dynamic> Sections { get; set; } = new List<dynamic>();
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
		Incoming, Outgoing, Center
	}

	public enum SpecialChatNode
	{
		No, TextInputErrorFallback
	}
}