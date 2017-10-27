using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ANAConversationStudio.UIHelpers
{
	public class ListToVisiblityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IEnumerable V)
				return V != null && V.Cast<object>().Count() > 0 ? Visibility.Collapsed : Visibility.Visible;
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
