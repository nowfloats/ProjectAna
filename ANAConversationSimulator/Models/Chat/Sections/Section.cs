using ANAConversationSimulator.Models;
using Windows.UI.Xaml;

namespace ANAConversationSimulator.Models.Chat
{
    public class Section : BaseEntity
    {
        public Section(string id, SectionTypeEnum sectionType, int delayInMs)
        {
            this._id = id;
            this.SectionType = sectionType;
            this.DelayInMs = delayInMs;
        }

        public SectionTypeEnum SectionType { get; set; }

        public int DelayInMs { get; set; } = 0;
        public bool Hidden { get; set; } = false;

        public MessageDirection Direction { get; set; }
        public string Title { get; set; } = null;
        public string Caption { get; set; } = null;
        public Section() { }
    }

    public enum SectionTypeEnum
    {
        Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml, Carousel, Typing
    };

    public enum MessageDirection { In, Out }
}