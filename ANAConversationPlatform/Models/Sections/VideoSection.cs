namespace ANAConversationPlatform.Models.Sections
{
	public class VideoSection : TitleCaptionSection
	{
		public int HeightInPixels { get; set; }
		public int WidthInPixels { get; set; }
		public string Url { get; set; }
		public double SizeInKb { get; set; }
		public int DurationInSec { get; set; }
	}
}