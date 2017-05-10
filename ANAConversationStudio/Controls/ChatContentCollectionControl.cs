using MongoDB.Bson;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Xceed.Wpf.Toolkit;

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
            //ItemAdded += ItemAddedEventHandler;
            //ItemDeleted += ItemDeletedEventHandler;
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems?.Count > 0)
                    {
                        var contentItem = e.NewItems.Cast<object>().First();
                        MongoHelper.Current.Contents.Add(contentItem as BaseContent);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems?.Count > 0)
                    {
                        var c = e.OldItems.Cast<object>().First() as BaseContent;
                        var content = MongoHelper.Current.Contents.FirstOrDefault(x => x._id == c._id);
                        if (content != null)
                            MongoHelper.Current.Contents.Remove(content);
                    }
                    break;
                default:
                    break;
            }
        }

        //private void ItemDeletedEventHandler(object sender, ItemEventArgs e)
        //{
        //    var c = e.Item as BaseContent;
        //    var content = MongoHelper.Current.Contents.FirstOrDefault(x => x._id == c._id);
        //    if (content != null)
        //        MongoHelper.Current.Contents.Remove(content);
        //}

        //private void ItemAddedEventHandler(object sender, ItemEventArgs e)
        //{
        //    MongoHelper.Current.Contents.Add(e.Item as BaseContent);
        //}

        private void ItemAddingEventHandler(object sender, ItemAddingEventArgs e)
        {
            PreProccessAddingItem(e.Item);
        }

        public void PreProccessAddingItem(object item)
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
                {
                    (item as SectionContent).SectionId = (ChatContentOwner as Section)._id;
                    //if (item is TextSectionContent tsItem)
                    //    tsItem.SectionText = (ChatContentOwner as Section).Alias;
                }
                else if (ChatContentOwner is Button && item is ButtonContent)
                {
                    (item as ButtonContent).ButtonId = (ChatContentOwner as Button)._id;
                    //(item as ButtonContent).ButtonText = (ChatContentOwner as Button).Alias;
                }
            }
        }
    }
}
