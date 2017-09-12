namespace ANAConversationPlatform.Models.Sections
{
	public class EmbeddedHtmlSection : TitleCaptionSection
	{
		public string Url { get; set; }
		public bool IsAuthenticationRequired { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}