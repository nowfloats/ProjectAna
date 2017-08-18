using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class AudioSection : TitleCaptionUrlSection
    {
        public AudioSection()
        {
            SectionType = SectionTypeEnum.Audio;
        }
    }

    public enum AudioFormatEnum
    {
        Mp3, Wav, Ogg
    }
}