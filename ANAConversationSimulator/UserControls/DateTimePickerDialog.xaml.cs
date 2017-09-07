using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ANAConversationSimulator.UserControls
{
	public sealed partial class DateTimePickerDialog : ContentDialog
	{
		public DateTimePickerDialog()
		{
			this.InitializeComponent();
			this.DataContext = this;
		}

		public bool IsDateVisible
		{
			get { return (bool)GetValue(IsDateVisibleProperty); }
			set { SetValue(IsDateVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsDateVisibleProperty = DependencyProperty.Register("IsDateVisible", typeof(bool), typeof(DateTimePickerDialog), new PropertyMetadata(false));

		public bool IsTimeVisible
		{
			get { return (bool)GetValue(IsTimeVisibleProperty); }
			set { SetValue(IsTimeVisibleProperty, value); }
		}

		public static readonly DependencyProperty IsTimeVisibleProperty = DependencyProperty.Register("IsTimeVisible", typeof(bool), typeof(DateTimePickerDialog), new PropertyMetadata(false));

		public DateTimeOffset ChoosenDate
		{
			get { return (DateTimeOffset)GetValue(ChoosenDateProperty); }
			set { SetValue(ChoosenDateProperty, value); }
		}
		public static readonly DependencyProperty ChoosenDateProperty = DependencyProperty.Register("ChoosenDate", typeof(DateTimeOffset), typeof(DateTimePickerDialog), new PropertyMetadata(DateTimeOffset.Now));

		public TimeSpan ChoosenTime
		{
			get { return (TimeSpan)GetValue(ChoosenTimeProperty); }
			set { SetValue(ChoosenTimeProperty, value); }
		}

		public static readonly DependencyProperty ChoosenTimeProperty = DependencyProperty.Register("ChoosenTime", typeof(TimeSpan), typeof(DateTimePickerDialog), new PropertyMetadata(DateTime.Now.TimeOfDay));
	}
}
