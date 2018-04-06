using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LAN_Chat
{
    class RelayCommand : ICommand
    {

        private Action<object> action;

        private Func<bool> func;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (func != null)
                return func();
            return true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public RelayCommand(Action<object> action, Func<bool> func)
        {
            this.action = action;
            this.func = func;
        }
    }
}
