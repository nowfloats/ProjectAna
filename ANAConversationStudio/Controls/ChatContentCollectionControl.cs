using MongoDB.Bson;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Xceed.Wpf.Toolkit;
using ANAConversationStudio.Models.Chat.Sections;

namespace ANAConversationStudio.Controls
{
    public class ChatContentCollectionControl : CollectionControl
    {
        public ChatNode ParentChatNode
        {
            get { return (ChatNode)GetValue(ParentChatNodeProperty); }
            set { SetValue(ParentChatNodeProperty, value); }
        }
        public static readonly DependencyProperty ParentChatNodeProperty = DependencyProperty.Register(nameof(ParentChatNode), typeof(ChatNode), typeof(ChatContentCollectionControl), new PropertyMetadata((s, e) =>
        {
            var control = s as ChatContentCollectionControl;
            if (e.NewValue == null)
                control.Visibility = Visibility.Collapsed;
            else
                control.Visibility = Visibility.Visible;
        }));
        public object ChatContentOwner
        {
            get { return (object)GetValue(ChatContentOwnerProperty); }
            set { SetValue(ChatContentOwnerProperty, value); }
        }
        public static readonly DependencyProperty ChatContentOwnerProperty = DependencyProperty.Register(nameof(ChatContentOwner), typeof(object), typeof(ChatContentCollectionControl), new PropertyMetadata((s, e) =>
        {
            var control = s as ChatContentCollectionControl;

            var choosenOwnerObject = e.NewValue;
            if (choosenOwnerObject != null)
            {
                var contentObject = Utilities.GetContentObjectV2(choosenOwnerObject);
                if (contentObject != null)
                {
                    var contentBank = Utilities.GetContentBank(choosenOwnerObject);
                    control.NewItemTypes = new List<Type>() { contentObject.GetType() };
                    control.Items.Clear();
                    control.ItemsSource = null;
                    control.ItemsSource = contentBank;
                    control.ItemsSourceType = contentBank.GetType();
                    control.Margin = new Thickness(10);
                    control.Visibility = Visibility.Visible;
                    if (control.Items.Count > 0)
                        control.SelectedItem = control.Items.First();
                    else
                        control.SelectedItem = null;
                }
                else
                    control.Visibility = Visibility.Collapsed;
            }
            else
                control.Visibility = Visibility.Collapsed;
        }));
        public ChatContentCollectionControl()
        {
            ItemAdding += ItemAddingEventHandler;
            ItemAdded += ItemAddedEventHandler;
            ItemDeleted += ItemDeletedEventHandler;
        }
        public void ContentItemAddExternal(BaseContent item)
        {
            if (item == null) return;
            PreProccessAddingItem(item);
            Items.Add(item);
            ContentItemAdded(item);
        }
        public void ContentItemDeleted(BaseContent item)
        {
            if (item == null) return;
            var content = MongoHelper.Current.Contents.FirstOrDefault(x => x._id == item._id);
            if (content != null)
                MongoHelper.Current.Contents.Remove(content);
        }

        private void ItemDeletedEventHandler(object sender, ItemEventArgs e)
        {
            ContentItemDeleted(e.Item as BaseContent);
        }
        private void ItemAddedEventHandler(object sender, ItemEventArgs e)
        {
            ContentItemAdded(e.Item as BaseContent);
        }
        private void ItemAddingEventHandler(object sender, ItemAddingEventArgs e)
        {
            PreProccessAddingItem(e.Item);
        }
        private void PreProccessAddingItem(object item)
        {
            if (item is BaseIdEntity)
                (item as BaseIdEntity)._id = ObjectId.GenerateNewId().ToString();
            if (item is BaseEntity)
                (item as BaseEntity).Alias = "New " + item.GetType().Name;

            if (item is BaseIdTimeStampEntity)
            {
                (item as BaseIdTimeStampEntity).CreatedOn = DateTime.Now;
                (item as BaseIdTimeStampEntity).UpdatedOn = DateTime.Now;
            }

            if (item is BaseContent && ParentChatNode != null)
                (item as BaseContent).NodeId = ParentChatNode.Id;

            if ((item is SectionContent || item is ButtonContent) && ChatContentOwner != null)
            {
                if (ChatContentOwner is Section && item is SectionContent)
                    (item as SectionContent).SectionId = (ChatContentOwner as Section)._id;
                else if (ChatContentOwner is Button && item is ButtonContent)
                    (item as ButtonContent).ButtonId = (ChatContentOwner as Button)._id;
            }

            if (item is CarouselButtonContent cBtnContent && ChatContentOwner != null && ChatContentOwner is CarouselButton cBtn)
                cBtnContent.CarouselButtonId = cBtn._id;

            if (item is CarouselItemContent cItemContent && ChatContentOwner != null && ChatContentOwner is CarouselItem cItem)
                cItemContent.CarouselItemId = cItem._id;
        }
        private void ContentItemAdded(BaseContent item)
        {
            if (item == null) return;
            MongoHelper.Current.Contents.Add(item as BaseContent);
        }
    }
}
