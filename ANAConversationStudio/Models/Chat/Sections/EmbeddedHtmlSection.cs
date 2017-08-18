using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class EmbeddedHtmlSection : TitleCaptionUrlSection
    {
        public EmbeddedHtmlSection()
        {
            SectionType = SectionTypeEnum.EmbeddedHtml;
        }
    }
}