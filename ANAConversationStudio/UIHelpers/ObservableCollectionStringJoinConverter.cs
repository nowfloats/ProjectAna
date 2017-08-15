using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ANAConversationStudio.UIHelpers
{
    public class ObservableCollectionStringJoinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<string> arrVal)
                return string.Join(", ", arrVal);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sValue)
                return new ObservableCollection<string>(sValue.Split(new[] { ",", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
            return value;
        }
    }
}
