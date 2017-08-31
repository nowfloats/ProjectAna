using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using ANAConversationStudio.Helpers;

namespace ANAConversationStudio.UserControls
{
    public partial class NodeEditorUserControl : System.Windows.Controls.UserControl
    {
        public NodeEditorUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ChatNode ChatNode
        {
            get { return (ChatNode)GetValue(ChatNodeProperty); }
            set { SetValue(ChatNodeProperty, value); }
        }
        public static readonly DependencyProperty ChatNodeProperty = DependencyProperty.Register("ChatNode", typeof(ChatNode), typeof(NodeEditorUserControl), new PropertyMetadata((s, e) =>
        {
            var editor = s as NodeEditorUserControl;
            var chatNode = e.NewValue as ChatNode;
            if (chatNode != null)
            {
                foreach (var section in chatNode.Sections)
                {
                    section.ParentNode = chatNode;

                    var carSec = section as CarouselSection;
                    if (carSec != null)
                    {
                        if (carSec.Items != null)
                            foreach (var carItem in carSec.Items)
                            {
                                carItem.ParentCarouselSection = carSec;
                                if (carItem.Buttons != null)
                                    foreach (var carBtn in carItem.Buttons)
                                        carBtn.ParentCarouselItem = carItem;
                            }
                    }
                }
                foreach (var button in chatNode.Buttons)
                    button.ParentNode = chatNode;
            }
        }));

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            ChatNode.AddSection.Execute((SectionTypeEnum)(sender as FrameworkElement).Tag);
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            ChatNode.AddButton.Execute(this);
        }

        #region Combo box item source
        public IEnumerable<Placement> PlacementOptions => Enum.GetValues(typeof(Placement)).Cast<Placement>();
        public IEnumerable<NodeTypeEnum> NodeTypes => Enum.GetValues(typeof(NodeTypeEnum)).Cast<NodeTypeEnum>();
        public IEnumerable<ApiMethodEnum> APIMethods => Enum.GetValues(typeof(ApiMethodEnum)).Cast<ApiMethodEnum>();
        #endregion

        private void CopyNodeIdClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ChatNode?.Id);
        }

        //public event EventHandler<ChatNode> NodeEditCommited;
        //private void DoneButtonClick(object sender, RoutedEventArgs e)
        //{
        //    NodeEditCommited?.Invoke(this, ChatNode);
        //}
    }

    public class NodeTypeToGroupVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var srcType = (NodeTypeEnum)value;
            var visibleOnlyFor = (NodeTypeEnum)parameter;
            return srcType == visibleOnlyFor ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ButtonTypeToFieldVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fieldName = parameter as string;
            var buttonType = (ButtonTypeEnum)value;
            var hidden = false;

            switch (buttonType)
            {
                case ButtonTypeEnum.PostText:
                    hidden = true; //Hide all. Probably!
                    break;
                case ButtonTypeEnum.OpenUrl:
                    hidden = !(new[] { nameof(Button.Url) }.Contains(fieldName));//Show only Url field
                    break;
                case ButtonTypeEnum.GetText:
                case ButtonTypeEnum.GetNumber:
                case ButtonTypeEnum.GetAddress:
                case ButtonTypeEnum.GetEmail:
                case ButtonTypeEnum.GetPhoneNumber:
                    //if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
                    hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.Url), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.GetTime:
                case ButtonTypeEnum.GetDate:
                case ButtonTypeEnum.GetDateTime:
                case ButtonTypeEnum.GetLocation:
                    hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.Url), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue), nameof(Button.PostfixText), nameof(Button.PrefixText) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.GetImage:
                case ButtonTypeEnum.GetAudio:
                case ButtonTypeEnum.GetVideo:
                    //if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
                    hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.PlaceholderText), nameof(Button.Url), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.GetItemFromSource:
                    hidden = new[] { nameof(Button.NextNodeId), nameof(Button.DeepLinkUrl), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.NextNode:
                    hidden = new[] { nameof(Button.NextNodeId), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.DeepLinkUrl), nameof(Button.Url), nameof(Button.PlaceholderText), }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.DeepLink:
                    hidden = new[] { nameof(Button.NextNodeId), nameof(Button.Url), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.PlaceholderText), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.GetAgent:
                    hidden = true; //Hide all. Probably!
                    break;
                case ButtonTypeEnum.ShowConfirmation:
                    hidden = true; //Hide all. Probably!
                    break;
                case ButtonTypeEnum.FetchChatFlow:
                    hidden = new[] { nameof(Button.DeepLinkUrl), nameof(Button.PlaceholderText), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue) }.Contains(fieldName);
                    break;
                default:
                    break;
            }

            return hidden ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
