using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sieve.Eratosthenes.ViewModels
{
    public class NaturalNumberViewModel : BaseViewModel
    {
        public Action<NaturalNumberViewModel> PrimeNumberSelected { get; set; }
        public NaturalNumberViewModel(int num, Action<NaturalNumberViewModel> selectedRoutine)
        {
            Number = num;
            PrimeNumberSelected = selectedRoutine;
        }
        #region Properties
        private int number;
        public int Number
        {
            get => number;
            set
            {
                number = value;
                NotifyPropertyChanged();
            }
        }

        private int compositeFoundSequence = 0;
        public int CompositeFoundSequence
        {
            get => compositeFoundSequence;
            set
            {
                compositeFoundSequence = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsPrime");
            }
        }

        public bool IsPrime
        {
            get => CompositeFoundSequence == 0;
        }

        private bool isSelectedPrime;
        public bool IsSelectedPrime
        {
            get => isSelectedPrime;
            set
            {
                isSelectedPrime = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Commands
        private ICommand showDetailProcessCommand;
        public ICommand ShowDetailProcessCommand
        {
            get => showDetailProcessCommand ?? (showDetailProcessCommand = new CommandHandler(() => ShowDetailProcessCommandAction(), true));
        }
        public void ShowDetailProcessCommandAction()
        {
            if (PrimeNumberSelected != null)
            {
                PrimeNumberSelected(this);
            }
        }
        #endregion
    }
}
