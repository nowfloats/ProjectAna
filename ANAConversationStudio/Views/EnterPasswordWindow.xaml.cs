using ANAConversationStudio.Helpers;
using System.Diagnostics;
using System.Windows;

namespace ANAConversationStudio.Views
{
    public partial class EnterPasswordWindow : Window
    {
        public EnterPasswordWindow()
        {
            InitializeComponent();
        }
        public bool Success { get; set; }
        private void SubmitClick(object sender, RoutedEventArgs e)
        {
            Submit();
        }
        private void Submit()
        {
            if (!string.IsNullOrWhiteSpace(PwdBox.Password))
            {
                try
                {
                    Utilities.Settings = Settings.Load(PwdBox.Password);
                    App.Cryptio = PwdBox.Password;
                    Success = true;
                    Close();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Entered password is incorrect! Please try again.", "Incorrect password");
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Success)
            {
                MainWindow.Current.AskAlert = false;
                Application.Current.Shutdown();
            }
        }

        private void PwdBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Submit();
        }

        private void ForgotPasswordClick(object sender, RoutedEventArgs e)
        {
            var msg = "Password protection is mainly set to secure the write access to your chat server which this studio has. \r\n\r\nIf you want to reset the password, your saved chat server connections will be deleted and you will have to configure them again. \r\n\r\nNote that your CHAT FLOWS WILL NOT BE DELETED, only the saved chat server connections will be deleted from this PC. \r\nYou just need to add the chat server connections again after you set the new password.\r\n\r\nDo you want to proceed?";
            if (MessageBox.Show(msg, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                Settings.Delete();

                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PwdBox.Focus();
        }
    }
}
