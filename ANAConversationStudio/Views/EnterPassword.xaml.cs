using ANAConversationStudio.Helpers;
using System.Windows;

namespace ANAConversationStudio.Views
{
    /// <summary>
    /// Interaction logic for EnterPassword.xaml
    /// </summary>
    public partial class EnterPassword : Window
    {
        public EnterPassword()
        {
            InitializeComponent();
#if DEBUG
            this.Loaded += (s, e) =>
            {
                PwdBox.Password = "nizam@123";
                Submit();
            };
#endif
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
    }
}
