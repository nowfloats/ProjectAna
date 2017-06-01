using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ANAConversationPlatform.Models.Sections
{
    public class Section : BaseEntity
    {
        public Section() { }
        public Section(string id, SectionTypeEnum sectionType, int delayInMs)
        {
            this._id = id;
            this.SectionType = sectionType;
            this.DelayInMs = delayInMs;
        }
        public SectionTypeEnum SectionType { get; set; }
        public int DelayInMs { get; set; } = 0;
        public bool Hidden { get; set; } = false;

        [BsonIgnore]
        [JsonIgnore]
        /// <summary>
        /// Used by the Client SDK to indicate the direction(In/Out) of a single chat section. Ignored here.
        /// </summary>
        public MessageDirection Direction { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        /// <summary>
        /// Used by the Client SDK to indicate the order of the chat. Ignored here.
        /// </summary>
        public int Sno { get; set; }
    }

    public enum SectionTypeEnum
    {
        Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml, Carousel
    };

    public enum MessageDirection { In, Out }
}