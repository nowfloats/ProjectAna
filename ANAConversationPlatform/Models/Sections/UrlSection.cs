namespace ANAConversationPlatform.Models.Sections
{
    public class UrlSection : Section
	{
		public string Url { get; set; }
		public bool IsAuthenticationRequired { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }

		public UrlSection(string id, int delayInMs, string url, bool isAuthenticationRequired, string username, string password) : base(id, SectionTypeEnum.Link, delayInMs)
		{
			this.Url = url;
			this.IsAuthenticationRequired = isAuthenticationRequired;
			this.Username = username;
			this.Password = password;
		}

		public UrlSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Link, delayInMs)
		{
		}
	}
}