using ANAConversationStudio.Controls;
using System.ComponentModel;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ANAConversationStudio.Models.Chat
{
    [CategoryOrder("Important", 1)]
    [CategoryOrder("For ButtonType Get[X]", 2)]
    [CategoryOrder("Misc", 3)]
    public class Button : BaseEntity
    {
        public Button() { }

        #region Important
        private ButtonTypeEnum _ButtonType = ButtonTypeEnum.NextNode;
        [Category("Important")]
        public ButtonTypeEnum ButtonType
        {
            get { return _ButtonType; }
            set
            {
                if (_ButtonType != value)
                {
                    _ButtonType = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region For ButtonType Get[X]
        private string _PlaceholderText;
        [Category("For ButtonType Get[X]")]
        public string PlaceholderText
        {
            get { return _PlaceholderText; }
            set
            {
                if (_PlaceholderText != value)
                {
                    _PlaceholderText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _VariableValue;
        [Category("For ButtonType Get[X]")]
        public string VariableValue
        {
            get { return _VariableValue; }
            set
            {
                if (_VariableValue != value)
                {
                    _VariableValue = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _PrefixText;
        [Category("For ButtonType Get[X]")]
        public string PrefixText
        {
            get { return _PrefixText; }
            set
            {
                if (_PrefixText != value)
                {
                    _PrefixText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _PostfixText;
        [Category("For ButtonType Get[X]")]
        public string PostfixText
        {
            get { return _PostfixText; }
            set
            {
                if (_PostfixText != value)
                {
                    _PostfixText = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Misc
        private bool _PostToChat = true;
        [Category("Misc")]
        public bool PostToChat
        {
            get { return _PostToChat; }
            set
            {
                if (_PostToChat != value)
                {
                    _PostToChat = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _APIResponseMatchKey;
        [Category("Misc")]
        public string APIResponseMatchKey
        {
            get { return _APIResponseMatchKey; }
            set
            {
                if (_APIResponseMatchKey != value)
                {
                    _APIResponseMatchKey = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _APIResponseMatchValue;
        [Category("Misc")]
        public string APIResponseMatchValue
        {
            get { return _APIResponseMatchValue; }
            set
            {
                if (_APIResponseMatchValue != value)
                {
                    _APIResponseMatchValue = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _DeepLinkUrl;
        [Category("Misc")]
        public string DeepLinkUrl
        {
            get { return _DeepLinkUrl; }
            set
            {
                if (_DeepLinkUrl != value)
                {
                    _DeepLinkUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _Url;
        [Category("Misc")]
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

        private int? _BounceTimeout;
        [Category("Misc")]
        public int? BounceTimeout
        {
            get { return _BounceTimeout; }
            set
            {
                if (_BounceTimeout != value)
                {
                    _BounceTimeout = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _DefaultButton;
        [Category("Misc")]
        public bool DefaultButton
        {
            get { return _DefaultButton; }
            set
            {
                if (_DefaultButton != value)
                {
                    _DefaultButton = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _Hidden;
        [Category("Misc")]
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

        private string _NextNodeId;
        [Editor(typeof(ReadonlyTextBoxEditor), typeof(ReadonlyTextBoxEditor))]
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

        public override string ToString()
        {
            return Alias;
        }
    }
    public enum ButtonTypeEnum
    {
        PostText,
        OpenUrl,
        GetText,
        GetNumber,
        GetAddress,
        GetEmail,
        GetPhoneNumber,
        GetItemFromSource,
        GetImage,
        GetAudio,
        GetVideo,
        NextNode,
        DeepLink,
        GetAgent,
    }
}