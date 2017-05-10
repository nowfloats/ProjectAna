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
using System.Threading.Tasks;

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
            _collectionControl.ItemAdding += ItemAdding;
            _collectionControl.ItemAdded += ItemAdded;
            _collectionControl.ItemDeleted += ItemCollectionChanged;
            _collectionControl.ItemMovedDown += ItemCollectionChanged;
            _collectionControl.ItemMovedUp += ItemCollectionChanged;

            Editor.Content = "Edit";
            Editor.Click += (s, e) =>
            {
                InvalidatePropertyGrid();
                MainWindow.Current.NodeCollectionControl = _collectionControl;
                if (MainWindow.Current.SectionButtonEditorLayoutAnchorable != null)
                    MainWindow.Current.SectionButtonEditorLayoutAnchorable.IsActive = true;
            };
            base.ResolveValueBinding(_propertyItem);
        }

        private void ItemAdded(object sender, ItemEventArgs e)
        {
            InvalidateSource();
            Task.Delay(1000).ContinueWith(async (s) =>
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    var editor = MainWindow.Current.ChatContentCollectionEditor;
                    if (editor != null && editor.NewItemTypes?.Count > 0 && editor.Items.Count == 0)
                    {
                        var newItem = Activator.CreateInstance(editor.NewItemTypes.First());
                        editor.PreProccessAddingItem(newItem);
                        editor.Items.Add(newItem);
                        editor.SelectedItem = newItem;
                    }
                });
            });
        }

        private void _collectionControl_Loaded(object sender, RoutedEventArgs e)
        {
            _collectionControl.PropertyGrid.PropertyValueChanged -= PropertyGrid_PropertyValueChanged;
            _collectionControl.PropertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
        }

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            InvalidateSource();

            if ((e.Source as PropertyGrid).SelectedObject is BaseEntity bEntity && (e.OriginalSource is PropertyItem pItem))
            {
                if (pItem.PropertyName == nameof(bEntity.Alias)) //If name of the property changed is 'Alias'
                {
                    var editor = MainWindow.Current.ChatContentCollectionEditor;
                    if (editor.SelectedItem != null) //copy alias to content only when 1 content is present
                    {
                        if (editor.SelectedItem is ButtonContent bContent && string.IsNullOrWhiteSpace(bContent.ButtonText))
                            bContent.ButtonText = e.NewValue as string;
                        else if (editor.SelectedItem is TextSectionContent txtContent && string.IsNullOrWhiteSpace(txtContent.SectionText))
                            txtContent.SectionText = e.NewValue as string;
                    }
                }
            }
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