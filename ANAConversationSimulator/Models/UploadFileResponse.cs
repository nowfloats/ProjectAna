namespace ANAConversationSimulator.Models
{
	public class Link
	{
		public string rel;
		public string href;
	}

	public class UploadFileResponse
	{
		public bool data;
		public Link[] links;
	}
}
