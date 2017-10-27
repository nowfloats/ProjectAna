using ANAConversationStudio.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ANAConversationStudio.Views
{
	public partial class PublishChatProjectWindow : Window
	{
		public PublishChatProjectWindow(string flowUrl)
		{
			InitializeComponent();
			this.DataContext = this;

			this.flowUrl = flowUrl;

			if (Utilities.Settings.PublishServers.Count <= 0)
				ShowPublishServerManager();
			else
				this.PublishServers = new ObservableCollection<PublishServer>(Utilities.Settings.PublishServers);
		}
		private string flowUrl;

		public PublishChatProject PublishChatProject
		{
			get { return (PublishChatProject)GetValue(PublishChatProjectProperty); }
			set { SetValue(PublishChatProjectProperty, value); }
		}
		public static readonly DependencyProperty PublishChatProjectProperty = DependencyProperty.Register("PublishChatProject", typeof(PublishChatProject), typeof(PublishServersManagerWindow), new PropertyMetadata(null));

		public PublishServer PublishServer
		{
			get { return (PublishServer)GetValue(PublishServerProperty); }
			set { SetValue(PublishServerProperty, value); }
		}
		public static readonly DependencyProperty PublishServerProperty = DependencyProperty.Register("PublishServer", typeof(PublishServer), typeof(PublishChatProjectWindow), new PropertyMetadata(null));

		private ObservableCollection<PublishServer> PublishServers
		{
			get { return (ObservableCollection<PublishServer>)GetValue(PublishServersProperty); }
			set { SetValue(PublishServersProperty, value); }
		}
		private static readonly DependencyProperty PublishServersProperty = DependencyProperty.Register("PublishServers", typeof(ObservableCollection<PublishServer>), typeof(PublishChatProjectWindow), new PropertyMetadata(null));

		private void ShowPublishServerManager()
		{
			new PublishServersManagerWindow().ShowDialog();
			this.PublishServers = new ObservableCollection<PublishServer>(Utilities.Settings.PublishServers);
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private async void PublishClick(object sender, RoutedEventArgs e)
		{
			if (PublishServer != null && PublishChatProject != null && PublishServer.ChatProjects.Contains(PublishChatProject))
			{
				var publishClient = new ChatProjectPublishClient(PublishServer);

				bool confirmed = false;

				(var exists, var msg) = await publishClient.ProjectExists(PublishChatProject);

				switch (exists)
				{
					case true:
						confirmed = MessageBox.Show($"A project is already published by the given chat project id!\r\n\r\nAre you sure you want to publish to the project {PublishChatProject}(Id: {PublishChatProject.Id}) on server {PublishServer}(URL: {PublishServer.Url})?. \r\nIt will override the existing chat project", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
						break;
					case false:
						confirmed = MessageBox.Show($"Are you sure you want to publish to the project {PublishChatProject}(Id: {PublishChatProject.Id}) on server {PublishServer}(URL: {PublishServer.Url})?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
						break;
					case null:
					default:
						{
							MessageBox.Show(msg, "Unable to determine the existence of the project!");
							return;
						}
				}

				if (confirmed)
				{
					(var status, var text) = await publishClient.Publish(PublishChatProject, this.flowUrl);
					if (status)
					{
						MessageBox.Show("Chat published successfully!", "Done");
						Close();
					}
					else
						MessageBox.Show(text, "Unable to publish the project");
				}
			}
			else
			{
				MessageBox.Show("No publish server and project selected", "Oops!");
			}
		}

		private void ManagePublishServersClick(object sender, RoutedEventArgs e)
		{
			ShowPublishServerManager();
		}
	}
}