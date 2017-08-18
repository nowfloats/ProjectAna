using ANAConversationStudio.Helpers;
using ANAConversationStudio.UIHelpers;
using MongoDB.Bson.Serialization.Attributes;
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
            Alias = Utilities.TrimText(SectionType + "");
        }


        private ChatNode _ParentNode;
        [JsonIgnore]
        [BsonIgnore]
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
        [BsonIgnore]
        public ICommand Remove => new ActionCommand((p) => ParentNode.Sections.Remove(this));

        [JsonIgnore]
        [BsonIgnore]
        public ICommand MoveUp => new ActionCommand((p) =>
        {
            var oldIdx = ParentNode.Sections.IndexOf(this);
            if (oldIdx <= 0) return;

            ParentNode.Sections.Move(oldIdx, oldIdx - 1);
        });

        [JsonIgnore]
        [BsonIgnore]
        public ICommand MoveDown => new ActionCommand((p) =>
        {
            var oldIdx = ParentNode.Sections.IndexOf(this);
            if (oldIdx >= ParentNode.Sections.Count - 1) return;

            ParentNode.Sections.Move(oldIdx, oldIdx + 1);
        });


        [JsonIgnore]
        [BsonIgnore]
        public string ContentId { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        public string ContentEmotion { get; set; }
    }

    public class TitleCaptionSection : Section
    {
        //Content
        private string _Title;
        [JsonIgnore]
        [BsonIgnore]
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
        [JsonIgnore]
        [BsonIgnore]
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
            var text = "";
            if (!string.IsNullOrWhiteSpace(Title))
                text = Title;
            else if (!string.IsNullOrWhiteSpace(Caption))
                text = Caption;
            else
                text = SectionType + "";

            Alias = Utilities.TrimText(text);
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