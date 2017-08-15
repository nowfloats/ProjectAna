using ANAConversationStudio.Helpers;
using ANAConversationStudio.UIHelpers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ANAConversationStudio.Models.Chat
{
    public class Button : RepeatableBaseEntity
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
        [JsonIgnore]
        [BsonIgnore]
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

        [JsonIgnore]
        [BsonIgnore]
        public IEnumerable<ButtonTypeEnum> ButtonTypes => Enum.GetValues(typeof(ButtonTypeEnum)).Cast<ButtonTypeEnum>();

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
        public ICommand Remove => new ActionCommand((p) => ParentNode.Buttons.Remove(this));

        [JsonIgnore]
        [BsonIgnore]
        public ICommand MoveUp => new ActionCommand((p) =>
        {
            var oldIdx = ParentNode.Buttons.IndexOf(this);
            if (oldIdx <= 0) return;

            ParentNode.Buttons.Move(oldIdx, oldIdx - 1);
        });

        [JsonIgnore]
        [BsonIgnore]
        public ICommand MoveDown => new ActionCommand((p) =>
        {
            var oldIdx = ParentNode.Buttons.IndexOf(this);
            if (oldIdx >= ParentNode.Buttons.Count - 1) return;

            ParentNode.Buttons.Move(oldIdx, oldIdx + 1);
        });

        [JsonIgnore]
        [BsonIgnore]
        public string ContentId { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        public string ContentEmotion { get; set; }

        private void FillAlias()
        {
            Alias = Utilities.TrimText(string.IsNullOrWhiteSpace(ButtonText) ? ButtonType + "" : ButtonText);
        }

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
        ShowConfirmation,
        FetchChatFlow
    }
}