using System.Windows;
using Xceed.Wpf.Toolkit;

namespace ANAConversationStudio.Controls
{
    public class ChatNodeCollectionControl : CollectionControl
    {
        public Models.Chat.ChatNode OwnerChatNode
        {
            get { return (Models.Chat.ChatNode)GetValue(OwnerChatNodeProperty); }
            set { SetValue(OwnerChatNodeProperty, value); }
        }
        public static readonly DependencyProperty OwnerChatNodeProperty = DependencyProperty.Register(nameof(OwnerChatNode), typeof(Models.Chat.ChatNode), typeof(ChatContentCollectionControl), new PropertyMetadata(null));
    }
}
