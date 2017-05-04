using ANAConversationSimulator.Helpers;
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
    public sealed partial class ShowMemoryStackDialog : ContentDialog
    {
        public ShowMemoryStackDialog()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public List<KeyValuePair<string, object>> MemoryStack
        {
            get { return (List<KeyValuePair<string, object>>)GetValue(MemoryStackProperty); }
            set { SetValue(MemoryStackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MemoryStack.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MemoryStackProperty =
            DependencyProperty.Register("MemoryStack", typeof(List<KeyValuePair<string, object>>), typeof(ShowMemoryStackDialog), new PropertyMetadata(null));

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            MemoryStack = Utils.LocalStore.ToList();
        }
    }
}
