using System.Windows;
using Xceed.Wpf.Toolkit;

namespace ANAConversationStudio.Views
{
    /// <summary>
    /// Interaction logic for ChatElementCollectionEditorWindow.xaml
    /// </summary>
    public partial class ChatElementCollectionEditorWindow : Window
    {
        public ChatElementCollectionEditorWindow()
        {
            InitializeComponent();
        }

        public ChatElementCollectionEditorWindow(CollectionControl control)
        {
            InitializeComponent();
            if (control.ItemsSourceType.GenericTypeArguments != null && control.ItemsSourceType.GenericTypeArguments.Length > 0)
                Title = $"Edit {control.ItemsSourceType.GenericTypeArguments[0].Name}";
            else
                Title = "Edit Element and its content";
            ElementEditor.Content = control;
        }
    }
}
