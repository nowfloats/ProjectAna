using ANAConversationSimulator.Models.Chat;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ANAConversationSimulator.UIHelpers
{
	class DirectionToBackgroundColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is MessageDirection direction)
			{
				switch (direction)
				{
					case MessageDirection.In:
						return (SolidColorBrush)App.Current.Resources["PaleWhiteBrush"];
					case MessageDirection.AwkwardCenter:
						return (SolidColorBrush)App.Current.Resources["PaleWhiteBrush"];
					case MessageDirection.Out:
						return new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]) { Opacity = 1 };
					default:
						return (SolidColorBrush)App.Current.Resources["PaleWhiteBrush"];
				}
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}
