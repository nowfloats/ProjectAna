namespace ANAConversationPlatform.Models.Sections
{
    public class EmbeddedHtmlSection : Section
	{
		public string Title { get; set; }
		public string Caption { get; set; }
		public string Url { get; set; }
		public bool IsAuthenticationRequired { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public EmbeddedHtmlSection(string id, string title, string caption, int delayInMs, string url, bool isAuthenticationRequired, string username, string password) : base(id, SectionTypeEnum.EmbeddedHtml, delayInMs)
		{
			this.Title = title;
			this.Caption = caption;
			this.Url = url;
			this.IsAuthenticationRequired = isAuthenticationRequired;
			this.Username = username;
			this.Password = password;
		}

		public EmbeddedHtmlSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.EmbeddedHtml, delayInMs)
		{
		}
	}
}