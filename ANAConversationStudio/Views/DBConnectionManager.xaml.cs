using MongoDB.Bson;
using ANAConversationStudio.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ANAConversationStudio.Views
{
    /// <summary>
    /// Interaction logic for DBConnectionManager.xaml
    /// </summary>
    public partial class DBConnectionManager : Window
    {
        public DBConnectionManager(List<DatabaseConnection> databaseConnections)
        {
            InitializeComponent();
            this.Closing += DBConnectionManager_Closing;
            CollectionControl.ItemsSource = databaseConnections.DeepCopy();
            CollectionControl.ItemsSourceType = typeof(ObservableCollection<DatabaseConnection>);
            CollectionControl.NewItemTypes = new List<Type>() { typeof(DatabaseConnection) };
        }

        private void DBConnectionManager_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (HideConfirm) return;
            if (!hasChanges()) return;
            var op = MessageBox.Show("Save changes?", "Hold on!", MessageBoxButton.YesNoCancel);
            if (op == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (op == MessageBoxResult.Yes)
            {
                Save();
            }
        }
        private bool HideConfirm = false;
        private void ConnectToSelectedClick(object sender, RoutedEventArgs e)
        {
            if (CollectionControl.SelectedItem as DatabaseConnection != null)
            {
                MongoHelper.Current = new MongoHelper(CollectionControl.SelectedItem as DatabaseConnection);
                MainWindow.Current.ViewModel.LoadNodesFromDB();
                MainWindow.Current.Status("Loaded");
                HideConfirm = true;
                Close();
            }
        }

        private bool hasChanges()
        {
            //var conns = CollectionControl.Items.Cast<DatabaseConnection>().ToList();
            //return Utilities.Settings.SavedConnections.ToJson() != conns.ToJson();
            return true;
        }

        private bool Save()
        {
            var conns = CollectionControl.Items.Cast<DatabaseConnection>().ToList();

            if (hasChanges())
            {
                if (conns.Any(x => !x.IsValid()))
                {
                    MessageBox.Show("The following connections are invalid, please correct them: \r\n" + string.Join("\r\n", conns.Where(x => !x.IsValid())), "Oops!");
                    return false;
                }
                Utilities.Settings.SavedConnections = conns;
                Utilities.Settings.Save(App.Cryptio);
            }
            return true;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (Save())
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

        private void SaveAndConnectClick(object sender, RoutedEventArgs e)
        {
            if (CollectionControl.SelectedItem as DatabaseConnection != null)
            {
                try
                {
                    if (!(CollectionControl.SelectedItem as DatabaseConnection).IsValid())
                    {
                        MessageBox.Show("The connection details are invalid. Please correct them and try again.", "Oops!");
                        return;
                    }
                    MongoHelper.Current = new MongoHelper(CollectionControl.SelectedItem as DatabaseConnection);
                    var connected = MainWindow.Current.ViewModel.LoadNodesFromDB();
                    if (connected)
                    {
                        MainWindow.Current.Status("Chat flow loaded");
                        if (Save())
                        {
                            HideConfirm = true;
                            Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MongoHelper.Current = null;
                    MainWindow.Current.Status("Unable to connect to the database");
                }
            }
        }
    }
}
