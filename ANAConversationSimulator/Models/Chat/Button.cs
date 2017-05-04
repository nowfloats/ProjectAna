using ANAConversationSimulator.Services.ChatInterfaceServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ANAConversationSimulator.Models.Chat
{
    public class Button : BaseEntity, INotifyPropertyChanged
    {
        public Button() { }

        public string ButtonName { get; set; }
        public string ButtonText { get; set; }
        public EmotionEnum Emotion { get; set; }
        public ButtonTypeEnum ButtonType { get; set; }
        public string DeepLinkUrl { get; set; }
        public string Url { get; set; }
        public int? BounceTimeout { get; set; } = null;
        public string NextNodeId { get; set; }
        public bool DefaultButton { get; set; } = false;
        public bool Hidden { get; set; }
        public string VariableValue { get; set; } = null;
        public string PlaceholderText { get; set; }
        public string PrefixText { get; set; }
        public string PostfixText { get; set; }

        private Dictionary<string, string> _items;
        /// <summary>
        /// Contains visible/filtered items in case of <see cref="ButtonTypeEnum.GetItemFromSource"/>
        /// </summary>
        public Dictionary<string, string> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Contains all items in case of <see cref="ButtonTypeEnum.GetItemFromSource"/>
        /// </summary>
        public Dictionary<string, string> ItemsSource { get; set; }

        //Below fields are filled at runtime
        public string VariableName { get; set; } = null;
        public ButtonActionCommand Action { get; } = new ButtonActionCommand();
        public bool Visible { get { return !Hidden; } }

        public ButtonKind Kind
        {
            get
            {
                switch (ButtonType)
                {
                    case ButtonTypeEnum.GetText:
                    case ButtonTypeEnum.GetEmail:
                    case ButtonTypeEnum.GetNumber:
                    case ButtonTypeEnum.GetPhoneNumber:
                    case ButtonTypeEnum.GetItemFromSource:
                        return ButtonKind.TextInput;
                    case ButtonTypeEnum.None:
                    case ButtonTypeEnum.PostText:
                    case ButtonTypeEnum.OpenUrl:
                    case ButtonTypeEnum.GetImage:
                    case ButtonTypeEnum.GetAudio:
                    case ButtonTypeEnum.GetVideo:
                    case ButtonTypeEnum.NextNode:
                    case ButtonTypeEnum.DeepLink:
                    case ButtonTypeEnum.GetAgent:
                    case ButtonTypeEnum.ApiCall:
                        return ButtonKind.ClickInput;
                    default:
                        return ButtonKind.ClickInput;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string pName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }
    }
    public enum ButtonKind
    {
        ClickInput, TextInput
    }
    public enum ButtonTypeEnum
    {
        None,
        PostText,
        OpenUrl,
        GetText,
        GetItemFromSource,
        GetAddress,
        GetEmail,
        GetPhoneNumber,
        GetNumber,
        GetImage,
        GetAudio,
        GetVideo,
        NextNode,
        DeepLink,
        GetAgent,
        ApiCall
    }
}