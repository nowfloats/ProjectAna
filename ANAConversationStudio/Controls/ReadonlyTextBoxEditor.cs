using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace ANAConversationStudio.Controls
{
    public class ReadonlyTextBoxEditor : TextBoxEditor
    {
        public override FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var editor = base.ResolveEditor(propertyItem);
            (editor as TextBox).IsReadOnly = true;
            (editor as TextBox).IsReadOnlyCaretVisible = true;
            return editor;
        }
    }
}
