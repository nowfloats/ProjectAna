namespace ANAConversationSimulator.Models.Chat.Sections
{
    public class VideoSection : Section
    {
        public int HeightInPixels { get; set; }
        public int WidthInPixels { get; set; }
        public string Url { get; set; }
        public double SizeInKb { get; set; }
        public int DurationInSec { get; set; }

        public VideoSection(string id, int delayInMs, string title, string caption, int heightInPixels, int widthInPixels, string url, double sizeInKb, int durationInSec) : base(id, SectionTypeEnum.Video, delayInMs)
        {
            this.Title = title;
            this.Caption = caption;
            this.HeightInPixels = heightInPixels;
            this.WidthInPixels = widthInPixels;
            this.Url = url;
            this.SizeInKb = sizeInKb;
            this.DurationInSec = durationInSec;
        }

        public VideoSection(string id, int delayInMs = 0) : base(id, SectionTypeEnum.Video, delayInMs)
        {
        }
        public VideoSection() { SectionType = SectionTypeEnum.Video; }
    }
}