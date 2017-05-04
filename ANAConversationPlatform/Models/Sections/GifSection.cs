namespace ANAConversationPlatform.Models.Sections
{
    public class GifSection : Section
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public bool PreFetch { get; set; }
		public string Caption { get; set; }

		public GifSection(string id, string title, int delayInMs, string url, bool preFetch, string caption) : base(id, SectionTypeEnum.Gif, delayInMs)
		{
			this.Title = title;
			this.Url = url;
			this.PreFetch = preFetch;
			this.Caption = caption;
		}

		public GifSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Gif, delayInMs)
		{
		}
	}
}