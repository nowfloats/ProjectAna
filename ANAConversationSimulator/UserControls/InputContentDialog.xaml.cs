using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ANAConversationSimulator.UserControls
{
    public sealed partial class InputContentDialog : ContentDialog
    {
        public InputContentDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string ChatFlowAPI
        {
            get { return (string)GetValue(ChatFlowAPIProperty); }
            set { SetValue(ChatFlowAPIProperty, value); }
        }
        public static readonly DependencyProperty ChatFlowAPIProperty = DependencyProperty.Register("ChatFlowAPI", typeof(string), typeof(InputContentDialog), new PropertyMetadata(null));

        public string UploadFileAPI
        {
            get { return (string)GetValue(UploadFileAPIProperty); }
            set { SetValue(UploadFileAPIProperty, value); }
        }
        public static readonly DependencyProperty UploadFileAPIProperty = DependencyProperty.Register("UploadFileAPI", typeof(string), typeof(InputContentDialog), new PropertyMetadata(null));

        public string ActivityTrackAPI
        {
            get { return (string)GetValue(ActivityTrackAPIProperty); }
            set { SetValue(ActivityTrackAPIProperty, value); }
        }
        public static readonly DependencyProperty ActivityTrackAPIProperty = DependencyProperty.Register("ActivityTrackAPI", typeof(string), typeof(InputContentDialog), new PropertyMetadata(null));

        public string SocketServer
        {
            get { return (string)GetValue(SocketServerProperty); }
            set { SetValue(SocketServerProperty, value); }
        }

        public static readonly DependencyProperty SocketServerProperty = DependencyProperty.Register("SocketServer", typeof(string), typeof(InputContentDialog), new PropertyMetadata(null));

        public bool Result = false;
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = false;
        }
    }
}
