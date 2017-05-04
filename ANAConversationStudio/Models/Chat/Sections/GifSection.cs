using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class GifSection : Section
    {
        [Category("Important")]
        public string Url { get; set; }
        public bool PreFetch { get; set; }

        public GifSection()
        {
            SectionType = SectionTypeEnum.Gif;
        }
    }
}