using ANAConversationSimulator.Models.Chat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ANAConversationSimulator.UIHelpers
{
    public class ChatBubbleTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is Section section)
                return Application.Current.Resources[$"{section.SectionType}SectionChatBubbleTemplate"] as DataTemplate;
            return null;
        }
    }
}
