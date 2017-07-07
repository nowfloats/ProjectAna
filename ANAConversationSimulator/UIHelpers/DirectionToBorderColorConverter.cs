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
    public class DirectionToBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageDirection direction)
            {
                switch (direction)
                {
                    case MessageDirection.In:
                        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 234, 237, 242)) { Opacity = 1 }; //Pale White
                    case MessageDirection.AwkwardCenter:
                        return new SolidColorBrush(Windows.UI.Colors.Transparent);
                    case MessageDirection.Out:
                        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 173, 239)) { Opacity = 1 }; //NF Yellow
                    default:
                        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 234, 237, 242)) { Opacity = 1 }; //Pale White
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
