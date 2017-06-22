using Sieve.Eratosthenes.Core;
using Sieve.Eratosthenes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sieve.Eratosthenes.ViewModels
{
    public class DetailViewModel : BaseViewModel
    {
        private CancellationTokenSource TaskCTS { get; set; }
        public ObservableCollection<NaturalNumberViewModel> Numbers { get; set; }
        public DemoPrimeModel PrimeM { get; set; }
        private int limit;

        public DetailViewModel()
        {
            Numbers = new ObservableCollection<NaturalNumberViewModel>();
            PrimeM = new DemoPrimeModel();
            TaskCTS = new CancellationTokenSource();
        }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private NaturalNumberViewModel selectedPrimeNumber;
        public NaturalNumberViewModel SelectedPrimeNumber
        {
            get => selectedPrimeNumber;
            set
            {
                selectedPrimeNumber = value;
                NotifyPropertyChanged();
                IsBusy = true;
                new NotifyTaskCompletion<bool>(TriggerGenerateneturalNumber(), new Action(() =>
                {
                    IsBusy = false;
                }));
            }
        }

        async Task<bool> TriggerGenerateneturalNumber()
        {
            await GenerateNaturalNumberRange();
            return await Task<bool>.Run(async () =>
            {
                await PrimeM.StraightSieveEratosthenesDemo(limit, Numbers, TaskCTS);
                return true;
            });
        }
        
        async Task GenerateNaturalNumberRange()
        {
            int startFrom = 2, i;
            if ((selectedPrimeNumber.Number - 50) >= 2)
            {
                startFrom = selectedPrimeNumber.Number - 50;
                int x = startFrom % 10;
                if ((startFrom - x) > 2)
                {
                    startFrom -= x;
                }
            }

            for (i = startFrom; i <= startFrom + 99; i++)
            {
                var naturalNumVM = new NaturalNumberViewModel(i, null);
                if (i == selectedPrimeNumber.Number)
                    naturalNumVM.IsSelectedPrime = true;
                Numbers.Add(naturalNumVM);
            }
            limit = i;
            await Task.Delay(1);
        }

        public void StopPrimeRangeShowOff()
        {
            TaskCTS.Cancel();
        }
    }
}
