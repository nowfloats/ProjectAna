using ANAConversationStudio.ViewModels;
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
	public class ConnectionToFillBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var srcConnectorParentNodeIsSelected = (bool)values[0];
				var destConnectorParentNodeIsSelected = (bool)values[1];

				return ((srcConnectorParentNodeIsSelected || destConnectorParentNodeIsSelected) ? Application.Current.Resources["connectionHighlightedBrush"] : Application.Current.Resources["connectionBrush"]);
			}
			catch (Exception ex)
			{
				return Application.Current.Resources["connectionBrush"];
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
