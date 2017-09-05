using ANAConversationStudio.Models.Chat;
using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace ANAConversationStudio.UIHelpers
{
    public class NodeTypeToGroupVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var srcType = (NodeTypeEnum)value;
            var visibleOnlyFor = (NodeTypeEnum)parameter;
            return srcType == visibleOnlyFor ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
