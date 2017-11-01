using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using Windows.UI.Xaml.Data;
using ANAConversationSimulator.Helpers;
using Windows.Foundation;
using ANAConversationSimulator.ViewModels;
using Windows.UI.ViewManagement;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.Storage;
using System.Net.Http;

namespace ANAConversationSimulator
{
	/// Documentation on APIs used in this page:
	/// https://github.com/Windows-XAML/Template10/wiki

	[Bindable]
	sealed partial class App : BootStrapper
	{
		public App()
		{
			InitializeComponent();
			SplashFactory = (e) => new Views.Splash(e);

			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
			{
				ApplicationView.PreferredLaunchViewSize = new Size(500, 600);
				ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
			}

			this.UnhandledException += App_UnhandledException;
		}

		private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			Utils.ShowDialog("Oops! Something went wrong.\r\n" + e.Exception.Message);
		}

		public override async Task OnInitializeAsync(IActivatedEventArgs args)
		{
			if (Window.Current.Content as ModalDialog == null)
			{
				// create a new frame 
				var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

				// create modal root
				Window.Current.Content = new ModalDialog
				{
					DisableBackButtonWhenModal = true,
					Content = new Views.Shell(nav),
					ModalContent = new Views.Busy(),
				};

				if (Utils.GetDeviceFormFactorType() == DeviceFormFactorType.Phone)//Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0)
					await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
			}
			await Task.CompletedTask;
		}

		public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
		{
			if (args is ProtocolActivatedEventArgs pArgs)
			{
				if (!string.IsNullOrWhiteSpace(pArgs.Uri.Query))
				{
					try
					{
						var parsedQuery = new WwwFormUrlDecoder(pArgs.Uri.Query);
						if (parsedQuery.Count > 0)
						{
							try
							{
								var chatUrl = parsedQuery.GetFirstValueByName("chatflow");
								using (var hc = new HttpClient())
									Utils.CurrentChatFlowJson = await hc.GetStringAsync(chatUrl);
							}
							catch (Exception ex)
							{
								await Utils.ShowDialogAsync(ex.Message);
							}
							MainPageViewModel.CurrentInstance?.StartChatting();
						}
					}
					catch { }
				}
			}
			if (args is FileActivatedEventArgs fArgs && fArgs.Files.Count > 0)
			{
				var file = fArgs.Files[0] as StorageFile;
				Utils.CurrentChatFlowJson = await FileIO.ReadTextAsync(file);
				MainPageViewModel.CurrentInstance?.StartChatting();
			}
			// long-running startup tasks go here
			await Utils.LoadConfig();

			NavigationService.Navigate(typeof(Views.MainPage));
			await Task.CompletedTask;
		}
	}
}

