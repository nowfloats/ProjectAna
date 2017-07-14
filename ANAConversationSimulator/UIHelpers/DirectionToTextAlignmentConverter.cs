using ANAConversationSimulator.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ANAConversationSimulator.UIHelpers
{
    public class DirectionToTextAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageDirection direction)
            {
                switch (direction)
                {
                    case MessageDirection.In:
                        return TextAlignment.Left;
                    case MessageDirection.Out:
                        return TextAlignment.Left;
                    case MessageDirection.AwkwardCenter:
                        return TextAlignment.Center;
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
