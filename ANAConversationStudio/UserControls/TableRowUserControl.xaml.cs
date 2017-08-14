using System.Windows;
using System.Windows.Controls;

namespace ANAConversationStudio.UserControls
{
    public partial class TableRowUserControl : UserControl
    {
        public TableRowUserControl()
        {
            InitializeComponent();
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(TableRowUserControl), new PropertyMetadata());

        public object Field
        {
            get { return (object)GetValue(FieldProperty); }
            set { SetValue(FieldProperty, value); }
        }

        public static readonly DependencyProperty FieldProperty = DependencyProperty.Register("Field", typeof(object), typeof(TableRowUserControl), new PropertyMetadata());
    }
}
