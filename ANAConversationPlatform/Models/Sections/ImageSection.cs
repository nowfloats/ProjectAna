namespace ANAConversationPlatform.Models.Sections
{
	public class ImageSection : TitleCaptionSection
	{
		public string AltText { get; set; }
		public int HeightInPixels { get; set; }
		public int WidthInPixels { get; set; }
		public string Url { get; set; }
		public double SizeInKb { get; set; }
	}
}