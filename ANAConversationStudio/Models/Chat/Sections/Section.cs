using ANAConversationStudio.Models;
using ANAConversationStudio.Helpers;
using System;
using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat
{
    public class Section : BaseEntity
    {
        public Section() { }

        [Category("Important")]
        public SectionTypeEnum SectionType { get; set; }

        public int DelayInMs { get; set; } = Utilities.Rand();
        public bool Hidden { get; set; } = false;
    }

    public enum SectionTypeEnum
    {
        Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml, Carousel
    };
}