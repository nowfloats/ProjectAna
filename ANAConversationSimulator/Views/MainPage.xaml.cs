using System;
using ANAConversationSimulator.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Coding4Fun.Toolkit.Controls;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
namespace ANAConversationSimulator.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            Loaded += MainPage_LoadedAsync;
        }

        private bool isLoaded;
        private void MainPage_LoadedAsync(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                var context = this.DataContext as MainPageViewModel;
                context.ChatThread.CollectionChanged += ChatThread_CollectionChanged;
                context.StartChatting();
                isLoaded = true;
            }
        }
        private async void ChatThread_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await Task.Delay(200).ContinueWith(async (x) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (ThreadScrollViewer.ScrollableHeight > 0)
                        ThreadScrollViewer.ChangeView(ThreadScrollViewer.HorizontalOffset, ThreadScrollViewer.ScrollableHeight, ThreadScrollViewer.ZoomFactor, true);
                });
            });
        }
    }
}
