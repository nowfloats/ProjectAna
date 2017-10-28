using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ANAConversationSimulator.UserControls
{
	public sealed partial class InputContentDialog : ContentDialog
	{
		public InputContentDialog()
		{
			InitializeComponent();
			DataContext = this;
		}

		public string UploadFileAPI
		{
			get { return (string)GetValue(UploadFileAPIProperty); }
			set { SetValue(UploadFileAPIProperty, value); }
		}
		public static readonly DependencyProperty UploadFileAPIProperty = DependencyProperty.Register("UploadFileAPI", typeof(string), typeof(InputContentDialog), new PropertyMetadata(null));

		public bool Result = false;
		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Result = true;
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Result = false;
		}
	}
}
