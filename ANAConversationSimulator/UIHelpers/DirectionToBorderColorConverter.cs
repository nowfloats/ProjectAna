using ANAConversationSimulator.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ANAConversationSimulator.UIHelpers
{
	public class DirectionToBorderColorConverter : IValueConverter
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
						return new SolidColorBrush(Windows.UI.Colors.Transparent);
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
