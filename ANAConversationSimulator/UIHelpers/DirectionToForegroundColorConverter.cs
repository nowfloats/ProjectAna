using ANAConversationSimulator.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ANAConversationSimulator.UIHelpers
{
    class DirectionToForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageDirection direction)
            {
                switch (direction)
                {
                    case MessageDirection.In:
                        return new SolidColorBrush(Windows.UI.Colors.Black) { Opacity = 1 }; 
                    case MessageDirection.Out:
                        return new SolidColorBrush(Windows.UI.Colors.White) { Opacity = 1 };
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
