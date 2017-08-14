using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class GifSection : TitleCaptionUrlSection
    {
        public GifSection()
        {
            SectionType = SectionTypeEnum.Gif;
        }
    }
}