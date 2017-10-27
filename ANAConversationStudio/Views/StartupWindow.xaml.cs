using ANAConversationStudio.Helpers;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ANAConversationStudio.Views
{
	/// <summary>
	/// Interaction logic for StartupWindow.xaml
	/// </summary>
	public partial class StartupWindow : Window
	{
		public StartupWindow()
		{
			InitializeComponent();
			this.DataContext = this;

			RecentProjectPaths = new ObservableCollection<string>(Utilities.Settings.RecentChatFlowFiles);
		}

		public ObservableCollection<string> RecentProjectPaths
		{
			get { return (ObservableCollection<string>)GetValue(RecentProjectPathsProperty); }
			set { SetValue(RecentProjectPathsProperty, value); }
		}
		public static readonly DependencyProperty RecentProjectPathsProperty = DependencyProperty.Register("RecentProjectPaths", typeof(ObservableCollection<string>), typeof(StartupWindow), new PropertyMetadata(null));

		private void OpenProjectClick(object sender, RoutedEventArgs e)
		{
			var loaded = StudioContext.Open(MainWindow.Current?.ViewModel);
			if (loaded)
				Close();
		}

		private void NewProjectClick(object sender, RoutedEventArgs e)
		{
			var loaded = StudioContext.LoadNew(MainWindow.Current?.ViewModel);
			if (loaded)
				Close();
		}

		private void RecentProjectSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var loaded = StudioContext.Load((sender as ListBox).SelectedItem as string, MainWindow.Current?.ViewModel);
			if (loaded)
				Close();
		}
	}
}
