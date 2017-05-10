using MongoDB.Bson;
using ANAConversationStudio.Views;
using System.ComponentModel;
using System.Linq;

namespace ANAConversationStudio.Models
{
    public class BaseContent : BaseIdTimeStampEntity
    {
        public BaseContent() { _id = ObjectId.GenerateNewId().ToString(); }

        public string NodeName
        {
            get
            {
                try
                {
                    if (MainWindow.Current != null)
                    {
                        var node = MainWindow.Current.ViewModel.Network.Nodes.FirstOrDefault(x => x.ChatNode.Id == NodeId);
                        if (node != null)
                            return node.Name;
                    }
                }
                catch { }
                return "";
            }
        }

        private string _NodeId;
        [ReadOnly(true)]
        public string NodeId
        {
            get { return _NodeId; }
            set
            {
                if (_NodeId != value)
                {
                    _NodeId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Emotion;
        public string Emotion
        {
            get { return _Emotion; }
            set
            {
                if (_Emotion != value)
                {
                    _Emotion = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class NodeContent : BaseContent
    {
        private string _NodeHeaderText;
        [Category("Important")]
        public string NodeHeaderText
        {
            get { return _NodeHeaderText; }
            set
            {
                if (_NodeHeaderText != value)
                {
                    _NodeHeaderText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class SectionContent : BaseContent
    {
        private string _SectionId;
        [ReadOnly(true)]
        public string SectionId
        {
            get { return _SectionId; }
            set
            {
                if (_SectionId != value)
                {
                    _SectionId = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class TextSectionContent : SectionContent
    {
        private string _SectionText;
        [Category("Important")]
        public string SectionText
        {
            get { return _SectionText; }
            set
            {
                if (_SectionText != value)
                {
                    _SectionText = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class TitleCaptionSectionContent : SectionContent
    {
        private string _Title;
        [Category("Important")]
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
        [Category("Important")]
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
    public class ImageSectionContent : TitleCaptionSectionContent
    {
        private string _AltText;
        public string AltText
        {
            get { return _AltText; }
            set
            {
                if (_AltText != value)
                {
                    _AltText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class GraphSectionContent : SectionContent
    {
        private string _XLabel;
        public string XLabel
        {
            get { return _XLabel; }
            set
            {
                if (_XLabel != value)
                {
                    _XLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _YLabel;
        public string YLabel
        {
            get { return _YLabel; }
            set
            {
                if (_YLabel != value)
                {
                    _YLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CoordinateListLegend;
        public string CoordinateListLegend
        {
            get { return _CoordinateListLegend; }
            set
            {
                if (_CoordinateListLegend != value)
                {
                    _CoordinateListLegend = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CoordinateListId;
        public string CoordinateListId
        {
            get { return _CoordinateListId; }
            set
            {
                if (_CoordinateListId != value)
                {
                    _CoordinateListId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CoordinateText;
        public string CoordinateText
        {
            get { return _CoordinateText; }
            set
            {
                if (_CoordinateText != value)
                {
                    _CoordinateText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class ButtonContent : BaseContent
    {
        private string _ButtonId;
        [ReadOnly(true)]
        public string ButtonId
        {
            get { return _ButtonId; }
            set
            {
                if (_ButtonId != value)
                {
                    _ButtonId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ButtonText;
        [Category("Important")]
        public string ButtonText
        {
            get { return _ButtonText; }
            set
            {
                if (_ButtonText != value)
                {
                    _ButtonText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}