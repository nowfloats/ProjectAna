using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace ANAConversationStudio.UIHelpers
{
	public class FilePathToTitleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string sVal)
				return Path.GetFileNameWithoutExtension(sVal);
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
