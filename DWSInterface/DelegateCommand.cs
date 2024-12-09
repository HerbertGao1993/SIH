using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DWSInterface
{
    class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public DelegateCommand(Action<object> action)
        {
            ExecuteAction = action;
        }
        public bool CanExecute(object parameter)
        {
            if (CanExcuteFunc == null)
            {
                return true;
            }
            return CanExcuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction == null)
            {
                return;
            }
            ExecuteAction(parameter);
        }

        public Action<object> ExecuteAction { get; set; }
        public Func<object, bool> CanExcuteFunc { get; set; }
    }
    
}
