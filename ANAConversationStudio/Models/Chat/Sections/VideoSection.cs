using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class VideoSection : Section
    {
        public int HeightInPixels { get; set; }
        public int WidthInPixels { get; set; }
        [Category("Important")]
        public string Url { get; set; }
        public double SizeInKb { get; set; }
        public int DurationInSec { get; set; }
        public VideoSection() { SectionType = SectionTypeEnum.Video; }
    }
}