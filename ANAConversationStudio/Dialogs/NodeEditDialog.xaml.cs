using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using MongoDB.Bson;

namespace ANAConversationStudio.Dialogs
{
    public partial class NodeEditDialog : Window
    {
        public NodeEditDialog(ChatNode chatNode)
        {
            InitializeComponent();
            #region Default Chat Node for testing
            //if (ChatNode == null)
            //{
            //    ChatNode = new ChatNode();
            //    ChatNode.Sections = new System.Collections.ObjectModel.ObservableCollection<Section>()
            //    {
            //        new TextSection{ Text = "Hello", ParentNode = ChatNode },
            //        new TextSection{ Text = "Bye",  ParentNode = ChatNode },
            //        new ImageSection { Title = "Title", Caption = "Caption", Url = "Url", ParentNode = ChatNode  },
            //        new ImageSection { Title = "Title 2", Caption = "Caption 3", Url = "Url 4" , ParentNode = ChatNode },
            //    };
            //    ChatNode.Buttons = new System.Collections.ObjectModel.ObservableCollection<Button>()
            //    {
            //        new Button{ ParentNode = ChatNode  },
            //        new Button{ParentNode = ChatNode  },
            //        new Button{ParentNode = ChatNode  }
            //    };
            //} 
            #endregion
            ChatNode = chatNode;
            DataContext = this;
        }

        public ChatNode ChatNode
        {
            get { return (ChatNode)GetValue(ChatNodeProperty); }
            set { SetValue(ChatNodeProperty, value); }
        }
        public static readonly DependencyProperty ChatNodeProperty = DependencyProperty.Register("ChatNode", typeof(ChatNode), typeof(NodeEditDialog), new PropertyMetadata((s, e) =>
        {
            var editor = s as NodeEditDialog;
            var chatNode = e.NewValue as ChatNode;
            if (chatNode != null)
            {
                foreach (var section in chatNode.Sections)
                    section.ParentNode = chatNode;
                foreach (var button in chatNode.Buttons)
                    button.ParentNode = chatNode;
            }
        }));

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            var type = (SectionTypeEnum)(sender as System.Windows.Controls.MenuItem).Tag;
            switch (type)
            {
                case SectionTypeEnum.Image:
                    ChatNode.Sections.Add(new ImageSection
                    {
                        Alias = "New Image",
                        Caption = "Image Caption",
                        Title = "Image Title",
                        Url = "http://",
                        _id = ObjectId.GenerateNewId().ToString(),
                    });
                    break;
                case SectionTypeEnum.Text:
                    ChatNode.Sections.Add(new TextSection
                    {
                        Alias = "New Text",
                        Text = "Text Section",
                        _id = ObjectId.GenerateNewId().ToString(),
                    });
                    break;
                case SectionTypeEnum.Gif:
                    ChatNode.Sections.Add(new GifSection
                    {
                        Alias = "New Gif",
                        Caption = "Gif Caption",
                        Title = "Gif Title",
                        Url = "http://",
                        _id = ObjectId.GenerateNewId().ToString(),
                    });
                    break;
                case SectionTypeEnum.Audio:
                    ChatNode.Sections.Add(new AudioSection
                    {
                        Alias = "New Audio",
                        Caption = "Audio Caption",
                        Title = "Audio Title",
                        Url = "http://",
                        _id = ObjectId.GenerateNewId().ToString(),
                    });
                    break;
                case SectionTypeEnum.Video:
                    ChatNode.Sections.Add(new VideoSection
                    {
                        Alias = "New Video",
                        Caption = "Video Caption",
                        Title = "Video Title",
                        Url = "http://",
                        _id = ObjectId.GenerateNewId().ToString(),
                    });
                    break;
                case SectionTypeEnum.EmbeddedHtml:
                    ChatNode.Sections.Add(new EmbeddedHtmlSection
                    {
                        Alias = "New Html",
                        Caption = "HTML Caption",
                        Title = "HTML Title",
                        Url = "http://",
                        _id = ObjectId.GenerateNewId().ToString(),
                    });
                    break;
                case SectionTypeEnum.Carousel:
                    MessageBox.Show("Coming soon");
                    //ChatNode.Sections.Add(new CarouselSection
                    //{
                    //    Alias = "New Image",
                    //    _id = ObjectId.GenerateNewId().ToString(),
                    //});
                    break;
                default:
                    break;
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            ChatNode.Buttons.Add(new Button
            {
                ParentNode = ChatNode,
                _id = ObjectId.GenerateNewId().ToString()
            });
        }
        
        #region Combo box item source
        public IEnumerable<Placement> PlacementOptions => Enum.GetValues(typeof(Placement)).Cast<Placement>();
        public IEnumerable<NodeTypeEnum> NodeTypes => Enum.GetValues(typeof(NodeTypeEnum)).Cast<NodeTypeEnum>();
        public string[] APIMethods => new[] { "GET", "POST" };
        #endregion
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

    public class ButtonSectionTypeSymbolConverter : IValueConverter
    {
        //Search the below as 'Segoe MDL2 Assets' in the internet to find how they look.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SectionTypeEnum secType)
            {
                switch (secType)
                {
                    case SectionTypeEnum.Image:
                        return "\uEB9F";
                    case SectionTypeEnum.Text:
                        return "\uE700";
                    case SectionTypeEnum.Graph:
                        break;
                    case SectionTypeEnum.Gif:
                        return "\uE913";
                    case SectionTypeEnum.Audio:
                        return "\uE8D6";
                    case SectionTypeEnum.Video:
                        return "\uE20A";
                    case SectionTypeEnum.Link:
                        break;
                    case SectionTypeEnum.EmbeddedHtml:
                        return "\uE12B";
                    case SectionTypeEnum.Carousel:
                        return "\uE89F";
                    case SectionTypeEnum.PrintOTP:
                        break;
                    default:
                        break;
                }
            }
            if (value is ButtonTypeEnum buttonType)
            {
                switch (buttonType)
                {
                    case ButtonTypeEnum.PostText:
                        break;
                    case ButtonTypeEnum.OpenUrl:
                        break;
                    case ButtonTypeEnum.GetText:
                        break;
                    case ButtonTypeEnum.GetNumber:
                        break;
                    case ButtonTypeEnum.GetAddress:
                        break;
                    case ButtonTypeEnum.GetEmail:
                        break;
                    case ButtonTypeEnum.GetPhoneNumber:
                        break;
                    case ButtonTypeEnum.GetItemFromSource:
                        break;
                    case ButtonTypeEnum.GetImage:
                        break;
                    case ButtonTypeEnum.GetAudio:
                        break;
                    case ButtonTypeEnum.GetVideo:
                        break;
                    case ButtonTypeEnum.NextNode:
                        break;
                    case ButtonTypeEnum.DeepLink:
                        break;
                    case ButtonTypeEnum.GetAgent:
                        break;
                    case ButtonTypeEnum.ShowConfirmation:
                        break;
                    case ButtonTypeEnum.FetchChatFlow:
                        break;
                    default:
                        break;
                }
            }
            return "\uE8F2";
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
                    hidden = new[] { nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue), nameof(Button.DeepLinkUrl), nameof(Button.Url) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.GetImage:
                case ButtonTypeEnum.GetAudio:
                case ButtonTypeEnum.GetVideo:
                    //if the passed button property is present in the list, that field should not be visible. here placeholder text should not be visible if button type is input(Get[X]) type
                    hidden = new[] { nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue), nameof(Button.DeepLinkUrl), nameof(Button.PlaceholderText), nameof(Button.Url), nameof(Button.PostfixText), nameof(Button.PrefixText) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.GetItemFromSource:
                    hidden = new[] { nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue), nameof(Button.DeepLinkUrl) }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.NextNode:
                    hidden = new[] { nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.DeepLinkUrl), nameof(Button.Url), nameof(Button.PlaceholderText), }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.DeepLink:
                    hidden = new[] { nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue), nameof(Button.Url), nameof(Button.PostfixText), nameof(Button.PrefixText), nameof(Button.PlaceholderText), }.Contains(fieldName);
                    break;
                case ButtonTypeEnum.GetAgent:
                    hidden = true; //Hide all. Probably!
                    break;
                case ButtonTypeEnum.ShowConfirmation:
                    hidden = true; //Hide all. Probably!
                    break;
                case ButtonTypeEnum.FetchChatFlow:
                    hidden = new[] { nameof(Button.APIResponseMatchKey), nameof(Button.APIResponseMatchValue), nameof(Button.DeepLinkUrl), nameof(Button.PlaceholderText), nameof(Button.PostfixText), nameof(Button.PrefixText) }.Contains(fieldName);
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
