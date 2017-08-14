using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class ImageSection : TitleCaptionUrlSection
    {
        public ImageSection()
        {
            SectionType = SectionTypeEnum.Image;
        }
    }
}