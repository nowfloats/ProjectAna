namespace ANAConversationPlatform.Models.Sections
{
    public class TextSection : Section
	{
		public string Text { get; set; }

		public TextSection(string id, int delayInMs, string text) : base(id, SectionTypeEnum.Text, delayInMs)
		{
			this.Text = text;
		}

		public TextSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Text, delayInMs)
		{
		}
	}
}