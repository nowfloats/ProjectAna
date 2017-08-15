using ANAConversationStudio.Controls;
using ANAConversationStudio.UIHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ANAConversationStudio.Models.Chat
{
    [CategoryOrder("Important", 1)]
    [CategoryOrder("For ButtonType Get[X]", 2)]
    [CategoryOrder("Misc", 3)]
    public class Button : BaseEntity
    {
        public Button()
        {
            FillAlias();
        }

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

                    FillAlias();
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

        #region For Repeat Buttons

        private bool _DoesRepeat;
        [Category("For Repeating Buttons")]
        public bool DoesRepeat
        {
            get { return _DoesRepeat; }
            set
            {
                if (_DoesRepeat != value)
                {
                    _DoesRepeat = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _RepeatOn;
        [Category("For Repeating Buttons")]
        public string RepeatOn
        {
            get { return _RepeatOn; }
            set
            {
                if (_RepeatOn != value)
                {
                    _RepeatOn = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _RepeatAs;
        [Category("For Repeating Buttons")]
        public string RepeatAs
        {
            get { return _RepeatAs; }
            set
            {
                if (_RepeatAs != value)
                {
                    _RepeatAs = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _StartPosition;
        [Category("For Repeating Buttons")]
        public int StartPosition
        {
            get { return _StartPosition; }
            set
            {
                if (_StartPosition != value)
                {
                    _StartPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _MaxRepeats;
        [Category("For Repeating Buttons")]
        public int MaxRepeats
        {
            get { return _MaxRepeats; }
            set
            {
                if (_MaxRepeats != value)
                {
                    _MaxRepeats = value;
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
        //[Editor(typeof(ReadonlyTextBoxEditor), typeof(ReadonlyTextBoxEditor))]
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

        //Content

        private string _ButtonText;
        public string ButtonText
        {
            get { return _ButtonText; }
            set
            {
                if (_ButtonText != value)
                {
                    _ButtonText = value;

                    FillAlias();
                    OnPropertyChanged();
                }
            }
        }

        private void FillAlias()
        {
            Alias = string.IsNullOrWhiteSpace(ButtonText) ? ButtonType + "" : ButtonText;
        }

        [JsonIgnore]
        public IEnumerable<ButtonTypeEnum> ButtonTypes => Enum.GetValues(typeof(ButtonTypeEnum)).Cast<ButtonTypeEnum>();

        public override string ToString()
        {
            return Alias;
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
        public ICommand Remove => new ActionCommand((p) => ParentNode.Buttons.Remove(this));
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
        ShowConfirmation,
        FetchChatFlow
    }
}