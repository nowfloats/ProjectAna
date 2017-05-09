using MongoDB.Bson;
using ANAConversationStudio.Models;
using ANAConversationStudio.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace ANAConversationStudio.Controls
{
    public class ChatNodeCollectionEditor<T> : TypeEditor<Button>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = CollectionControlButton.ItemsSourceProperty;
        }
        private PropertyItem _propertyItem;
        private ChatNodeCollectionControl _collectionControl;
        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            _propertyItem = propertyItem;
            _collectionControl = new ChatNodeCollectionControl()
            {
                ItemsSource = _propertyItem.Value as IEnumerable,
                ItemsSourceType = _propertyItem.PropertyType,
                Margin = new Thickness(10),
                OwnerChatNode = propertyItem.Instance as Models.Chat.ChatNode,
                NewItemTypes = _propertyItem.PropertyDescriptor.Attributes.Cast<Attribute>().Where(x => x is NewItemTypesAttribute).Cast<NewItemTypesAttribute>().SelectMany(x => x.Types).ToList()
            };

            if (_collectionControl.NewItemTypes == null || _collectionControl.NewItemTypes.Count == 0)
            {
                if (_propertyItem.PropertyType.BaseType == typeof(Array))
                    _collectionControl.NewItemTypes = new List<Type>() { _propertyItem.PropertyType.GetElementType() };
                else if (_propertyItem.PropertyType.GetGenericArguments().Count() > 0)
                    _collectionControl.NewItemTypes = new List<Type>() { _propertyItem.PropertyType.GetGenericArguments()[0] };
            }

            _collectionControl.Loaded += _collectionControl_Loaded;
            _collectionControl.ItemAdding += ItemAdding; ;
            _collectionControl.ItemAdded += ItemCollectionChanged;
            _collectionControl.ItemDeleted += ItemCollectionChanged;
            _collectionControl.ItemMovedDown += ItemCollectionChanged;
            _collectionControl.ItemMovedUp += ItemCollectionChanged;

            Editor.Content = "Edit";
            Editor.Click += (s, e) =>
            {
                InvalidatePropertyGrid();
                MainWindow.Current.NodeCollectionControl = _collectionControl;
            };
            base.ResolveValueBinding(_propertyItem);
        }

        private void _collectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            _collectionControl.PropertyGrid.PropertyValueChanged -= PropertyGrid_PropertyValueChanged;
            _collectionControl.PropertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        }

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            InvalidateSource();
        }
        private void InvalidatePropertyGrid()
        {
            if (_collectionControl.PropertyGrid != null && _collectionControl.PropertyGrid.SelectedObject != null)
                _collectionControl.PropertyGrid.Update();
        }
        private void InvalidateSource()
        {
            _propertyItem.Value = new ObservableCollection<T>(_collectionControl.Items.Cast<T>());
        }

        private void ItemAdding(object sender, ItemAddingEventArgs e)
        {
            var Item = e.Item;

            if (Item is BaseIdEntity)
                (Item as BaseIdEntity)._id = ObjectId.GenerateNewId().ToString();
            if (Item is BaseEntity)
                (Item as BaseEntity).Alias = "New " + Item.GetType().Name;
        }

        private void ItemCollectionChanged(object sender, ItemEventArgs e)
        {
            InvalidateSource();
        }
    }
}