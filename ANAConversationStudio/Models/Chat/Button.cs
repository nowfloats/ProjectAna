using System.ComponentModel;

namespace ANAConversationStudio.Models.Chat
{
    public class Button : BaseEntity
    {
        public Button() { }

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

        private string _DeepLinkUrl;
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

        private bool _DefaultButton;
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

        private string _VariableValue;
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

        private string _PlaceholderText;
        [Category("Important")]
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

        private bool _ConfirmInput;
        public bool ConfirmInput
        {
            get { return _ConfirmInput; }
            set
            {
                if (_ConfirmInput != value)
                {
                    _ConfirmInput = value;
                    OnPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return Alias;
        }
    }
    public enum ButtonTypeEnum
    {
        None,
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
        ApiCall
    }
}