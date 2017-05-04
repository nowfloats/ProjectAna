using ANAConversationSimulator.Helpers;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ANAConversationSimulator.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MemoryStackPage : Page
    {
        public MemoryStackPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public string PageTitle => "Memory Stack";

        public List<KeyValuePair<string, object>> MemoryStack
        {
            get { return (List<KeyValuePair<string, object>>)GetValue(MemoryStackProperty); }
            set { SetValue(MemoryStackProperty, value); }
        }
        public static readonly DependencyProperty MemoryStackProperty = DependencyProperty.Register("MemoryStack", typeof(List<KeyValuePair<string, object>>), typeof(MemoryStackPage), new PropertyMetadata(null));

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MemoryStack = Utils.LocalStore.ToList();
        }
    }
}
