using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ANAConversationStudio.UIHelpers
{
    public class ActionCommand : ICommand
    {
        private Action<object> _action;
        public ActionCommand(Action<object> action)
        {
            _action = action;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _action != null;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
