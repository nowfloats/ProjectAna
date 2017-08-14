using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class VideoSection : TitleCaptionUrlSection
    {
        public VideoSection() { SectionType = SectionTypeEnum.Video; }
    }
}