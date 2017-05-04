using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class AudioSection : Section
    {
        public AudioSection()
        {
            SectionType = SectionTypeEnum.Audio;
        }

        public int DurationInSeconds { get; set; }
        public AudioFormatEnum Format { get; set; }
        [Category("Important")]
        public string Url { get; set; }
        public string EncodingType { get; set; } = null;
        public bool Downloadable { get; set; } = false;
        public bool Buffer { get; set; } = false;
        public bool AnimateEmotion { get; set; } = false;
    }

    public enum AudioFormatEnum
    {
        Mp3, Wav, Ogg
    }
}