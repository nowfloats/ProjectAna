using ANAConversationStudio.Helpers;
using System.Windows;

namespace ANAConversationStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Logger.Write(e.Exception);
            MessageBox.Show(e.Exception.StackTrace, e.Exception.Message);
        }
        public static string Cryptio { get; set; }
    }
}
