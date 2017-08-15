using ANAConversationStudio.UIHelpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ANAConversationStudio.Models.Chat.Sections
{
    public class Section : BaseEntity
    {
        public Section()
        {
            FillAlias();
        }

        private SectionTypeEnum _SectionType;
        public SectionTypeEnum SectionType
        {
            get { return _SectionType; }
            set
            {
                if (_SectionType != value)
                {
                    _SectionType = value;

                    FillAlias();
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

        protected virtual void FillAlias()
        {
            Alias = SectionType + "";
        }


        private ChatNode _ParentNode;
        [JsonIgnore]
        public ChatNode ParentNode
        {
            get { return _ParentNode; }
            set
            {
                if (_ParentNode != value)
                {
                    _ParentNode = value;
                    OnPropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public ICommand Remove => new ActionCommand((p) => ParentNode.Sections.Remove(this));
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

                    FillAlias();
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

                    FillAlias();
                    OnPropertyChanged();
                }
            }
        }

        protected override void FillAlias()
        {
            var a = new List<string>();
            if (!string.IsNullOrWhiteSpace(Title))
                a.Add(Title);
            if (!string.IsNullOrWhiteSpace(Caption))
                a.Add(Caption);

            Alias = a.Count > 0 ? string.Join(" - ", a) : SectionType + "";
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