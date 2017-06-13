using ANAConversationStudio.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ANAConversationStudio.Views
{
    /// <summary>
    /// Interaction logic for SetPassword.xaml
    /// </summary>
    public partial class SetPassword : Window
    {
        public SetPassword()
        {
            InitializeComponent();
            this.Closing += SetPassword_Closing;
        }

        private void SetPassword_Closing(object sender, CancelEventArgs e)
        {
            if (!Success)
            {
                MainWindow.Current.AskAlert = false;
                App.Current.Shutdown();
            }
        }

        public bool Success { get; set; }
        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            Submit();
        }

        private void Submit()
        {
            if (!string.IsNullOrWhiteSpace(NewPasswordBox.Password) && NewPasswordBox.Password == ConfirmPasswordBox.Password)
            {
                App.Cryptio = NewPasswordBox.Password;
                if (Utilities.Settings == null)
                    Utilities.Settings = new Settings();
                Utilities.Settings.Save(App.Cryptio);
                Utilities.Settings = Settings.Load(App.Cryptio);
                Success = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Password cannot be empty or your new password does not match with confirm password!. Please correct it.", "Oops!");
            }
        }

        private void ConfirmPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Submit();
        }
    }
}
