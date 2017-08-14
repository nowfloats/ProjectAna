using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using MongoDB.Bson;

namespace ANAConversationStudio.Dialogs
{
    /// <summary>
    /// Interaction logic for NodeEditDialog.xaml
    /// </summary>
    public partial class NodeEditDialog : Window
    {
        public NodeEditDialog()
        {
            InitializeComponent();
            if (ChatNode == null)
            {
                ChatNode = new ChatNode()
                {
                    Sections = new System.Collections.ObjectModel.ObservableCollection<Section>()
                    {
                        new TextSection{ Text = "Hello",  },
                        new TextSection{ Text = "Bye",  },
                        new ImageSection { Title = "Title", Caption = "Caption", Url = "Url" },
                        new ImageSection { Title = "Title 2", Caption = "Caption 3", Url = "Url 4" },
                    }
                };
            }
            DataContext = this;
        }

        public ChatNode ChatNode
        {
            get { return (ChatNode)GetValue(ChatNodeProperty); }
            set { SetValue(ChatNodeProperty, value); }
        }
        public static readonly DependencyProperty ChatNodeProperty = DependencyProperty.Register("ChatNode", typeof(ChatNode), typeof(NodeEditDialog), new PropertyMetadata());

        #region Combo box item source
        public IEnumerable<Placement> PlacementOptions => Enum.GetValues(typeof(Placement)).Cast<Placement>();
        public IEnumerable<NodeTypeEnum> NodeTypes => Enum.GetValues(typeof(NodeTypeEnum)).Cast<NodeTypeEnum>();
        public string[] APIMethods => new[] { "GET", "POST" };
        #endregion

        private void TestButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void DialogUnloaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddSectionClick(object sender, RoutedEventArgs e)
        {
            var type = (SectionTypeEnum)(sender as MenuItem).Tag;
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

    public class SectionTypeSymbolConverter : IValueConverter
    {
        //Search the below as 'Segoe MDL2 Assets' in the internet to find how they look.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var secType = (SectionTypeEnum)value;
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
            return "\uE8F2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
