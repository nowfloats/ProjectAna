using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class EmbeddedHtmlSection : Section
    {
        [Category("Important")]
        public string Url { get; set; }
        public bool IsAuthenticationRequired { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public EmbeddedHtmlSection()
        {
            SectionType = SectionTypeEnum.EmbeddedHtml;
        }
    }
}