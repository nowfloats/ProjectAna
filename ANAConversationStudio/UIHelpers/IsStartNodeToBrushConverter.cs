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
    public class IsStartNodeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isStartNode = (bool)value;

            return isStartNode ? Application.Current.Resources["startNodeHeaderBrush"] : Application.Current.Resources["nodeHeaderBorderBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
