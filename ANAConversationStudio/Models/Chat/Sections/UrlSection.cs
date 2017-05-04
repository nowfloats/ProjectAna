using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class UrlSection : Section
    {
        [Category("Important")]
        public string Url { get; set; }
        public bool IsAuthenticationRequired { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UrlSection()
        {
            SectionType = SectionTypeEnum.Link;
        }
    }
}