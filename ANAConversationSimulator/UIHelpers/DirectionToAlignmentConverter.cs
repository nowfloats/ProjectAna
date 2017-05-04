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
    public class DirectionToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageDirection direction)
            {
                switch (direction)
                {
                    case MessageDirection.In:
                        return HorizontalAlignment.Left;
                    case MessageDirection.Out:
                        return HorizontalAlignment.Right;

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
