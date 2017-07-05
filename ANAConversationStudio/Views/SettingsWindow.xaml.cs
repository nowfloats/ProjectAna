using ANAConversationStudio.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ANAConversationStudio.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public EditableSettings Config
        {
            get { return (EditableSettings)GetValue(ConfigProperty); }
            set { SetValue(ConfigProperty, value); }
        }
        public static readonly DependencyProperty ConfigProperty = DependencyProperty.Register("Config", typeof(EditableSettings), typeof(SettingsWindow), new PropertyMetadata(null));

        public SettingsWindow(EditableSettings config)
        {
            InitializeComponent();
            this.DataContext = this;
            Config = config.DeepCopy();
        }
        public bool Save { get; set; }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            Save = true;
            this.Close();
        }
    }
}
