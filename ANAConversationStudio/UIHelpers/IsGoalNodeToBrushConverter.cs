using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ANAConversationStudio.UIHelpers
{
	public class IsGoalNodeToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isGoalNode = (bool)value;

			return isGoalNode ? Application.Current.Resources["ANAGreenBrush"] : new SolidColorBrush(Colors.Transparent);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
