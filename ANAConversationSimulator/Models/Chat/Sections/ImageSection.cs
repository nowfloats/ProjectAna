namespace ANAConversationSimulator.Models.Chat.Sections
{
    public class ImageSection : Section
    {
        public string AltText { get; set; }
        public int HeightInPixels { get; set; }
        public int WidthInPixels { get; set; }
        public string Url { get; set; }
        public double SizeInKb { get; set; }

        public ImageSection(string id, int delayInMs, string title, string caption, string altText, int heightInPixels, int widthInPixels, string url, double sizeInKb) : base(id, SectionTypeEnum.Image, delayInMs)
        {
            this.Title = title;
            this.Caption = caption;
            this.AltText = altText;
            this.HeightInPixels = heightInPixels;
            this.WidthInPixels = widthInPixels;
            this.Url = url;
            this.SizeInKb = sizeInKb;
        }

        public ImageSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Image, delayInMs)
        {
        }
        public ImageSection() { SectionType = SectionTypeEnum.Image; }
    }
}