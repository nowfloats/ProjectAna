using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class Section : BaseEntity
    {
        public Section() { }

        private SectionTypeEnum _SectionType;
        public SectionTypeEnum SectionType
        {
            get { return _SectionType; }
            set
            {
                if (_SectionType != value)
                {
                    _SectionType = value;
                    OnPropertyChanged();
                }
            }
        }


        private int _DelayInMs;
        public int DelayInMs
        {
            get { return _DelayInMs; }
            set
            {
                if (_DelayInMs != value)
                {
                    _DelayInMs = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _Hidden;
        public bool Hidden
        {
            get { return _Hidden; }
            set
            {
                if (_Hidden != value)
                {
                    _Hidden = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class TitleCaptionSection : Section
    {
        //Content
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Caption;
        public string Caption
        {
            get { return _Caption; }
            set
            {
                if (_Caption != value)
                {
                    _Caption = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class TitleCaptionUrlSection : TitleCaptionSection
    {
        private string _Url;
        public string Url
        {
            get { return _Url; }
            set
            {
                if (_Url != value)
                {
                    _Url = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public enum SectionTypeEnum
    {
        Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml, Carousel, PrintOTP
    };
}