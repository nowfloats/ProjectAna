using ANAConversationStudio.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ANAConversationStudio.Views
{
	public partial class PublishServersManagerWindow : Window
	{
		public PublishServersManagerWindow()
		{
			InitializeComponent();

			CollectionControl.PropertyGrid.ShowSearchBox = false;
			CollectionControl.PropertyGrid.ShowSortOptions = false;
			CollectionControl.PropertyGrid.ShowPreview = false;
			CollectionControl.PropertyGrid.ShowDescriptionByTooltip = false;
			CollectionControl.PropertyGrid.UpdateTextBoxSourceOnEnterKey = true;

			CollectionControl.ItemsSource = Utilities.Settings.PublishServers.DeepCopy();
			CollectionControl.ItemsSourceType = typeof(ObservableCollection<PublishServer>);
			CollectionControl.NewItemTypes = new List<Type>() { typeof(PublishServer) };

			LastSavedHash = Utilities.GenerateHash(JsonConvert.SerializeObject(Utilities.Settings.PublishServers));
		}
		private string LastSavedHash { get; set; }

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (HideConfirm) return;

			if (!string.IsNullOrWhiteSpace(LastSavedHash))
			{
				var currentHash = Utilities.GenerateHash(JsonConvert.SerializeObject(CollectionControl.ItemsSource as List<PublishServer>));
				if (currentHash == LastSavedHash) return; //no changes detected.
			}

			var op = MessageBox.Show("Save changes and exit?", "Hold on!", MessageBoxButton.YesNoCancel);
			if (op == MessageBoxResult.Yes)
			{
				Save();
				HideConfirm = true;
			}
			else if (op == MessageBoxResult.No)
				HideConfirm = true;
			else if (op == MessageBoxResult.Cancel)
				e.Cancel = true;
		}
		private bool HideConfirm = false;

		private bool Save()
		{
			var servers = CollectionControl.Items.Cast<PublishServer>().ToList();
			var serverValidations = servers.Select(x => x.Validate()).ToList();
			var invalidServers = serverValidations.Where(x => !x.Valid).ToList();

			if (invalidServers.Count > 0)
			{
				MessageBox.Show("The following publish server details are invalid, please correct them: \r\n\r\n" + string.Join("\r\n\r\n", invalidServers.Select(x => x.Msg)), "Oops!");
				return false;
			}
			Utilities.Settings.PublishServers = servers;
			LastSavedHash = Utilities.GenerateHash(JsonConvert.SerializeObject(Utilities.Settings.PublishServers));
			Utilities.Settings.Save(App.Cryptio);
			return true;
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			HideConfirm = true;
			Close();
		}

		private void SaveClick(object sender, RoutedEventArgs e)
		{
			var done = Save();
			if (done)
			{
				HideConfirm = true;
				Close();
			}
		}
	}
}