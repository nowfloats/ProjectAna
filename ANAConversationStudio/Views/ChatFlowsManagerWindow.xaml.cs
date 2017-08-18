using ANAConversationStudio.Helpers;
using ANAConversationStudio.Models.Chat;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ANAConversationStudio.Views
{
    public partial class ChatFlowsManagerWindow : Window
    {
        public ChatFlowsManagerWindow()
        {
            InitializeComponent();

            CollectionControl.ItemsSource = StudioContext.Current.ChatFlowProjects.DeepCopy();
            CollectionControl.ItemsSourceType = typeof(ObservableCollection<ANAProject>);
            CollectionControl.NewItemTypes = new List<Type>() { typeof(ANAProject) };
            CollectionControl.ItemAdding += CollectionControl_ItemAdding;
            CollectionControl.ItemDeleting += CollectionControl_ItemDeleting;
        }

        private void CollectionControl_ItemDeleting(object sender, Xceed.Wpf.Toolkit.ItemDeletingEventArgs e)
        {
            e.Cancel = true;
            MessageBox.Show("You do not have the access to delete a chat flow", "Access Denied");
        }

        private void CollectionControl_ItemAdding(object sender, Xceed.Wpf.Toolkit.ItemAddingEventArgs e)
        {
            if (e.Item is ANAProject aProj && string.IsNullOrWhiteSpace(aProj._id))
                aProj._id = ObjectId.GenerateNewId().ToString();
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            if (HideConfirm) return;

            var op = MessageBox.Show("Save changes?", "Hold on!", MessageBoxButton.YesNoCancel);
            if (op == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (op == MessageBoxResult.Yes)
            {
                await SaveAsync();
                Application.Current.Shutdown();
            }
        }
        private bool HideConfirm = false;

        private async Task<bool> SaveAsync()
        {
            var conns = CollectionControl.Items.Cast<ANAProject>().ToList();

            if (conns.Any(x => !x.IsValid()))
            {
                MessageBox.Show("The following chat flows are invalid, please correct them: \r\n" + string.Join("\r\n", conns.Where(x => !x.IsValid())), "Oops!");
                return false;
            }
            StudioContext.Current.ChatFlowProjects = conns;
            await StudioContext.Current.SaveProjectsAsync();
            CollectionControl.Items.Clear();
            CollectionControl.ItemsSource = StudioContext.Current.ChatFlowProjects.DeepCopy();
            MainWindow.Current?.LoadFileMenuSavedConnections();
            return true;
        }

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            if (await SaveAsync())
            {
                HideConfirm = true;
                Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            HideConfirm = true;
            Close();
        }

        private async void SaveAndConnectClick(object sender, RoutedEventArgs e)
        {
            var conn = (CollectionControl.SelectedItem as ANAProject);
            if (conn != null)
            {
                try
                {
                    if (!conn.IsValid())
                    {
                        MessageBox.Show("The chat flow details are invalid. Please correct them and try again.", "Oops!");
                        return;
                    }
                    if (await SaveAsync())
                    {
                        await MainWindow.Current.LoadProjectAsync(conn);
                        HideConfirm = true;
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load the selected chat flow project.");
                }
            }
        }
    }
}