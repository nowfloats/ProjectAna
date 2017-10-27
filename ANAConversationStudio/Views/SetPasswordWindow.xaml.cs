using ANAConversationStudio.Helpers;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ANAConversationStudio.Views
{
	public partial class SetPasswordWindow : Window
	{
		public SetPasswordWindow()
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
