using ANAConversationSimulator.Models.Chat;
using System;
using System.Linq;
using System.Windows.Input;

namespace ANAConversationSimulator.UIHelpers
{
    public class AutoSuggestFilterItemsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool canExecute = true;
        public bool CanExecute(object parameter)
        {
            if (parameter is Button button)
            {
                var val = button.VariableValue;
                if (button.ItemsSource == null) return false;
                var can = button.ItemsSource.Any(x => x.Value.ToLower().Contains(val.ToLower()));
                if (can != canExecute)
                {
                    canExecute = can;
                    CanExecuteChanged?.Invoke(this, new EventArgs());
                }
                return can;
            }
            return canExecute;
        }

        public void Execute(object parameter)
        {
            if (parameter is Button button)
            {
                var val = button.VariableValue;
                if (button.ItemsSource == null) return;
                button.Items = button.ItemsSource.Where(x => x.Value.ToLower().Contains(val.ToLower())).ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
