using Coding4Fun.Toolkit.Controls;
using ANAConversationSimulator.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ANAConversationSimulator.UIHelpers
{
    public class DirectionToBubbleDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MessageDirection direction)
            {
                switch (direction)
                {
                    case MessageDirection.In:
                        return ChatBubbleDirection.UpperLeft;
                    case MessageDirection.Out:
                        return ChatBubbleDirection.LowerRight;
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
