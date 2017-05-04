using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ANAConversationSimulator.UserControls
{
    public sealed partial class InputContentDialog : ContentDialog
    {
        public InputContentDialog()
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.Loaded += InputContentDialog_Loaded;
        }

        private void InputContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ChatFlowAPI))
                ChatFlowAPI = "http://<your-server.com>/api/Conversation/Chat";
            if (string.IsNullOrWhiteSpace(UploadFileAPI))
                UploadFileAPI = "http://<your-server.com>/api/Services/ReceiveFile?fileName={fileName}";
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
