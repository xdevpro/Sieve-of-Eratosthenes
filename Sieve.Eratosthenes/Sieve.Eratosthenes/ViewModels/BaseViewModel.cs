using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sieve.Eratosthenes.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Member
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class CommandHandler : ICommand
    {
        private Action commandAction;
        private bool commandCanExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            commandAction = action;
            commandCanExecute = canExecute;
        }

        #region ICommand members
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return commandCanExecute;
        }

        public void Execute(object parameter)
        {
            commandAction();
        }
        #endregion
    }
}
