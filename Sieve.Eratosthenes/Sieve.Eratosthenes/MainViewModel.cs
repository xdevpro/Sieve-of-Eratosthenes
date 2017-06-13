using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sieve.Eratosthenes
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            PrimeNumbers = new ObservableCollection<int>();
        }

        #region INotifyPropertyChanged Member
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Properties

        public ObservableCollection<int> PrimeNumbers { get; set; }

        private int maxRange = 5000;
        public int MaxRange
        {
            get => maxRange;
            set
            {
                maxRange = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        private ICommand startCommand;
        public ICommand StartCommand
        {
            get => startCommand ?? (startCommand = new CommandHandler(() => StartCommandAction(), true));
        }

        public void StartCommandAction()
        {
            StraightSieveEratosthenes();
        }

        void StraightSieveEratosthenes()
        {
            //BitArray primeCandidates = new BitArray(maxRange + 1);
            bool[] primeCandidates = new bool[maxRange + 1];
            int p, j;
            for (p = 2; p * p <= maxRange; p++)
            {
                // If primeCandidates[p] is not changed, then it is a prime
                if (!primeCandidates[p])
                {
                    PrimeNumbers.Add(p);
                    //await Task.Delay(100);
                    //Update all multiples of p
                    for (int i = p * 2; i <= maxRange; i += p)
                        primeCandidates[i] = true;
                }
            }
            for (j = p; j <= MaxRange; j++)
            {
                if (!primeCandidates[j]) PrimeNumbers.Add(j);
            }
        }

        //void SegmentedSieveEratosthenes()
        //{
        //    int limit = (int)Math.Floor(Math.Sqrt(MaxRange)) + 1;
        //    Vector<int> prime;

        //}

        private ICommand stopCommand;
        public ICommand StopCommand
        {
            get => stopCommand ?? (stopCommand = new CommandHandler(() => StopCommandAction(), true));
        }

        public void StopCommandAction()
        {
           
        }
        #endregion
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
