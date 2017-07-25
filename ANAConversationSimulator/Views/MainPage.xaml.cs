using System;
using ANAConversationSimulator.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Storage;
using ANAConversationSimulator.UserControls;
using ANAConversationSimulator.Helpers;

namespace ANAConversationSimulator.Views
{
    public sealed partial class MainPage : Page
    {
        public static MainPage Current { get; set; }
        public MainPage()
        {
            Current = this;
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

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

        public async void Reset()
        {
            await SaveChatAsImage();
            this.ViewModel.StartChatting();
        }

        public async void SaveChatAsImageWithName()
        {
            TextInputDialog d = new TextInputDialog()
            {
                Title = "Save Snapshot As..",
                PlaceholderText = "Enter a name to save the snapshot"
            };
            await d.ShowAsync();
            var saved = await SaveChatAsImage(Utils.NormalizeFileName(d.Text));
            if (!saved)
                await Utils.ShowDialogAsync("Unable to save the snapshot!");
            else
                await Utils.ShowDialogAsync($"The snapshot is saved in your Pictures/{ANACHATFLOWS_FOLDER} folder");
        }

        public async Task SaveChatAsImageDefault()
        {
            var saved = await SaveChatAsImage();
            if (!saved)
                await Utils.ShowDialogAsync("Unable to save the snapshot!");
            else
                await Utils.ShowDialogAsync($"The snapshot is saved in your \"Pictures/{ANACHATFLOWS_FOLDER}\" folder");
        }

        private const string ANACHATFLOWS_FOLDER = "ANA Chat Flows Snapshots";
        private async Task<bool> SaveChatAsImage(string fileName = null)
        {
            try
            {
                if (fileName == null)
                    fileName = "";

                fileName += " " + DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss tt") + ".png";

                StorageFolder sFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync(ANACHATFLOWS_FOLDER, CreationCollisionOption.OpenIfExists);
                var sFile = await sFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                await Utils.SaveVisualElementToFile(ThreadScrollViewer.Content as ItemsControl, sFile);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

/*
 private async void SaveChatAsImage()
        {
            RenderTargetBitmap renderTarget = new RenderTargetBitmap();
            await renderTarget.RenderAsync(ThreadScrollViewer, (int)ThreadScrollViewer.ScrollableWidth, (int)ThreadScrollViewer.ScrollableHeight);

            IBuffer pixelBuffer = await renderTarget.GetPixelsAsync();

            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            var buffer = await renderTarget.GetPixelsAsync();

            WriteableBitmap img = new WriteableBitmap(renderTarget.PixelWidth, renderTarget.PixelHeight);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)renderTarget.PixelWidth, (uint)renderTarget.PixelHeight, DisplayInformation.GetForCurrentView().LogicalDpi, DisplayInformation.GetForCurrentView().LogicalDpi, buffer.ToArray());
            await encoder.FlushAsync();

            await img.SetSourceAsync(stream);

            //SoftwareBitmap bitmap = SoftwareBitmap.CreateCopyFromBuffer(pixelBuffer, BitmapPixelFormat.Bgra8, (int)ThreadScrollViewer.ScrollableWidth, (int)ThreadScrollViewer.ScrollableHeight, BitmapAlphaMode.Premultiplied);

            StorageFolder sFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("ANA Chat Flows", CreationCollisionOption.OpenIfExists);
            var sFile = await sFolder.CreateFileAsync((DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + ".png"), CreationCollisionOption.GenerateUniqueName);
            using (var fileStream = await sFile.OpenStreamForWriteAsync())
            using (var s = img.PixelBuffer.AsStream())
                s.CopyTo(fileStream);
        }
     */
