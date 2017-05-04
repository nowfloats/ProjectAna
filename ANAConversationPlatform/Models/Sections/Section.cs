namespace ANAConversationPlatform.Models.Sections
{
    public class Section : BaseEntity
	{
		public Section(string id, SectionTypeEnum sectionType, int delayInMs)
		{
			this._id = id;
			this.SectionType = sectionType;
			this.DelayInMs = delayInMs;
		}
		public SectionTypeEnum SectionType { get; set; }
		public int DelayInMs { get; set; } = 0;
		public bool Hidden { get; set; } = false;
	}

	public enum SectionTypeEnum
	{
		Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml
	};
}