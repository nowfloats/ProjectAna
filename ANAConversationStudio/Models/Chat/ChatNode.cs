using MongoDB.Bson;
using ANAConversationStudio.Models.Chat.Sections;
using ANAConversationStudio.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ANAConversationStudio.Models.Chat
{
    public class ChatNode : INotifyPropertyChanged
    {
        public ChatNode()
        {
            Buttons.CollectionChanged += ButtonsCollectionChanged;
        }

        private string _Name;
        [Category("Important")]
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

        private ObservableCollection<Section> _Sections = new ObservableCollection<Section>();
        [NewItemTypes(typeof(TextSection), typeof(GifSection), typeof(ImageSection), typeof(GraphSection), typeof(AudioSection), typeof(VideoSection))]
        [Category("Important")]
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

        [Category("Important")]
        private string _VariableName;
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
        [Category("Important")]
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

        private string _Id;
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

        private int _TimeoutInMs = 15000;
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

        private NodeTypeEnum _NodeType = NodeTypeEnum.Combination;
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

        private DisplayTypeEnum _DisplayType = DisplayTypeEnum.Inline;
        public DisplayTypeEnum DisplayType
        {
            get { return _DisplayType; }
            set
            {
                if (_DisplayType != value)
                {
                    _DisplayType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ApiMethod;
        public string ApiMethod
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
                    item.Tag = "New Button";
                }
        }
    }

    public enum DisplayTypeEnum
    {
        Inline,
        FullscreenButtonList,
        FullscreenApiCall
    }

    public enum NodeTypeEnum
    {
        Image, Text, Graph, Gif, Audio, Video, Link, EmbeddedHtml, ApiCall, Combination
    };

    public enum EmotionEnum
    {
        Cool, Happy, Excited, Neutral, Sad, Irritated, Angry
    };
}