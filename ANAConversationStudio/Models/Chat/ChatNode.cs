using MongoDB.Bson;
using ANAConversationStudio.Models.Chat.Sections;
using ANAConversationStudio.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ANAConversationStudio.Models.Chat
{
    [CategoryOrder("Important", 1)]
    [CategoryOrder("For NodeType ApiCall", 2)]
    [CategoryOrder("For NodeType Card", 3)]
    [CategoryOrder("Misc", 4)]
    public class ChatNode : INotifyPropertyChanged
    {
        public ChatNode()
        {
            Buttons.CollectionChanged += ButtonsCollectionChanged;
        }

        #region Important
        private string _Name;
        [Category("Important")]
        [PropertyOrder(1)]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged();
                }
            }
        }

        private NodeTypeEnum _NodeType = NodeTypeEnum.Combination;
        [Category("Important")]
        [PropertyOrder(2)]
        public NodeTypeEnum NodeType
        {
            get { return _NodeType; }
            set
            {
                if (_NodeType != value)
                {
                    _NodeType = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Section> _Sections = new ObservableCollection<Section>();
        [NewItemTypes(typeof(TextSection), typeof(GifSection), typeof(ImageSection), typeof(EmbeddedHtmlSection), typeof(GraphSection), typeof(AudioSection), typeof(VideoSection), typeof(CarouselSection), typeof(PrintOTPSection))]
        [Category("Important")]
        [PropertyOrder(3)]
        [Editor(typeof(ChatNodeCollectionEditor<Section>), typeof(ChatNodeCollectionEditor<Section>))]
        public ObservableCollection<Section> Sections
        {
            get { return _Sections; }
            set
            {
                if (_Sections != value)
                {
                    _Sections = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Button> _Buttons = new ObservableCollection<Button>();
        [Category("Important")]
        [Editor(typeof(ChatNodeCollectionEditor<Button>), typeof(ChatNodeCollectionEditor<Button>))]
        [PropertyOrder(4)]
        public ObservableCollection<Button> Buttons
        {
            get { return _Buttons; }
            set
            {
                if (_Buttons != value)
                {
                    _Buttons = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Misc
        private string _Id;
        [Category("Misc")]
        [Editor(typeof(ReadonlyTextBoxEditor), typeof(ReadonlyTextBoxEditor))]
        public string Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _IsStartNode;
        [Category("Misc")]
        public bool IsStartNode
        {
            get { return _IsStartNode; }
            set
            {
                if (_IsStartNode != value)
                {
                    _IsStartNode = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _TimeoutInMs;
        [Category("Misc")]
        public int TimeoutInMs
        {
            get { return _TimeoutInMs; }
            set
            {
                if (_TimeoutInMs != value)
                {
                    _TimeoutInMs = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _GroupName;
        [Category("Misc")]
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                if (_GroupName != value)
                {
                    _GroupName = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region For NodeType ApiCall
        private string _VariableName;
        [Category("For NodeType ApiCall")]
        public string VariableName
        {
            get { return _VariableName; }
            set
            {
                if (_VariableName != value)
                {
                    _VariableName = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<string> _RequiredVariables;
        [Category("For NodeType ApiCall")]
        public ObservableCollection<string> RequiredVariables
        {
            get { return _RequiredVariables; }
            set
            {
                if (_RequiredVariables != value)
                {
                    _RequiredVariables = value;
                    OnPropertyChanged();
                }
            }
        }

        private ApiMethodEnum? _ApiMethod;
        [Category("For NodeType ApiCall")]
        public ApiMethodEnum? ApiMethod
        {
            get { return _ApiMethod; }
            set
            {
                if (_ApiMethod != value)
                {
                    _ApiMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ApiUrl;
        [Category("For NodeType ApiCall")]
        public string ApiUrl
        {
            get { return _ApiUrl; }
            set
            {
                if (_ApiUrl != value)
                {
                    _ApiUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _NextNodeId;
        [Category("For NodeType ApiCall")]
        public string NextNodeId
        {
            get { return _NextNodeId; }
            set
            {
                if (_NextNodeId != value)
                {
                    _NextNodeId = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region For NodeType Card
        private string _CardHeader;
        [Category("For NodeType Card")]
        [PropertyOrder(11)]
        public string CardHeader
        {
            get { return _CardHeader; }
            set
            {
                if (_CardHeader != value)
                {
                    _CardHeader = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CardFooter;
        [Category("For NodeType Card")]
        [PropertyOrder(12)]
        public string CardFooter
        {
            get { return _CardFooter; }
            set
            {
                if (_CardFooter != value)
                {
                    _CardFooter = value;
                    OnPropertyChanged();
                }
            }
        }

        private Placement? _Placement;
        [Category("For NodeType Card")]
        [PropertyOrder(13)]
        public Placement? Placement
        {
            get { return _Placement; }
            set
            {
                if (_Placement != value)
                {
                    _Placement = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public override string ToString()
        {
            return Name;
        }

        private void ButtonsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Button item in e.NewItems)
                {
                    item._id = ObjectId.GenerateNewId().ToString();
                    item.Alias = "New Button";
                }
        }
    }

    public enum ApiMethodEnum
    {
        GET, POST, PUT, HEAD, DELETE, OPTIONS, CONNECT
    }

    public enum NodeTypeEnum
    {
        ApiCall, Combination, Card
    };

    public enum EmotionEnum
    {
        Cool, Happy, Excited, Neutral, Sad, Irritated, Angry
    };

    public enum Placement
    {
        Incoming, Outgoing, Center
    }
}