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
    public class AutoSuggestFilterItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is Dictionary<string, string> dict)
            {
                var val = value as string;
                return dict.Where(x => x.Value.Contains(val)).ToDictionary(x => x, x => x);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
