using Sieve.Eratosthenes.Core;
using Sieve.Eratosthenes.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace Sieve.Eratosthenes.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<NaturalNumberViewModel> DisplayablePrimeNumbers { get; set; }
        public PrimeModel PrimeM { get; set; }
        public NotifyTaskCompletion<bool> PrimeNumberProcess { get; set; }  

        private DetailViewModel detailVM;
        public DetailViewModel DetailVM
        {
            get => detailVM;
            set
            {
                detailVM = value;
                NotifyPropertyChanged();
            }
        }

        public MainViewModel()
        {
            PrimeM = new PrimeModel();
            DisplayablePrimeNumbers = new ObservableCollection<NaturalNumberViewModel>();
            Range = 300;
            DelayProcessCommandAction();
        }

        public void PrimeNumberSelectedNotified(NaturalNumberViewModel vm)
        {
            IsDetailViewVisible = true;
            DetailVM = new DetailViewModel();
            DetailVM.SelectedPrimeNumber = vm;
        }

        #region Properties
        private double adjustedBoundHeight;
        public double AdjustedBoundHeight
        {
            get => adjustedBoundHeight;
            set
            {
                adjustedBoundHeight = value;
                NotifyPropertyChanged();
            }
        }

        private double adjustedBoundWidth;
        public double AdjustedBoundWidth
        {
            get => adjustedBoundWidth;
            set
            {
                adjustedBoundWidth = value;
                NotifyPropertyChanged();
            }
        }

        public int DisplayablePrimeNumberCount
        {
            get => (int)((Math.Floor(AdjustedBoundHeight) / 53) * (Math.Floor(AdjustedBoundWidth) / 53));
        }
        
        private int maxRange = 10000;
        public int MaxRange
        {
            get => maxRange;
            set
            {
                maxRange = value;
                NotifyPropertyChanged();
            }
        }
        
        private int minRange = 2;
        public int MinRange
        {
            get => minRange;
            set
            {
                if (value < minRange)
                {
                    int delta = minRange - value;
                    if (range - delta > 0)
                    {
                        range -= delta;
                        minRange = value;
                    }
                    else
                    {
                        range = range - 1;
                        minRange = 2;
                    }
                    IsDelayedBackwardProcessing = true;
                }
                else if (value > minRange)
                {
                    int delta = value - minRange;
                    if (range + delta <= maxRange)
                    {
                        range += delta;
                        IsDelayedProcessing = true;
                        minRange = value;
                    }
                    else
                    {
                        range = MaxRange;
                    }
                }
                
                NotifyPropertyChanged("Range");
                NotifyPropertyChanged();
            }
        }

        private int range;
        public int Range
        {
            get => range;
            set
            {
                if (value < range)
                {
                    int delta = range - value;
                    minRange -= (minRange - delta > 0) ? delta : minRange - 1;
                }
                else if (value > range)
                {
                    int delta = value - range;
                    minRange += delta;
                }
                range = value;
                IsDelayedProcessing = true;
                NotifyPropertyChanged("MinRange");
                NotifyPropertyChanged();
            }
        }
        
        bool IsDelayedProcessing { get; set; }
        bool IsDelayedBackwardProcessing { get; set; }
        
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

        private bool isDetailViewVisible;
        public bool IsDetailViewVisible
        {
            get => isDetailViewVisible;
            set
            {
                isDetailViewVisible = value;
                NotifyPropertyChanged();
                if (!value)
                    DetailVM.StopPrimeRangeShowOff();
            }
        }

        #endregion
        public async Task<bool> AdjustPrimeNumberRangesBackward()
        {
            return await Task<bool>.Run(async () =>
            {
                await RearrangeRange();
                return true;
            });
        }

        private async Task RearrangeRange()
        {
            int nextPrimeNumber = minRange;
            while (!PrimeM.PrimeNumbers.Contains(++nextPrimeNumber)) ;
            int i = PrimeM.PrimeNumbers.IndexOf(nextPrimeNumber);
            if (i >= 0)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       for (int j = 0; j <= DisplayablePrimeNumberCount && i < PrimeM.PrimeNumbers.Count; j++, i++)
                       {
                           if (j < DisplayablePrimeNumbers.Count)
                           {
                               DisplayablePrimeNumbers[j].Number = PrimeM.PrimeNumbers[i];
                           }
                           else
                           {
                               DisplayablePrimeNumbers.Add(new NaturalNumberViewModel(PrimeM.PrimeNumbers[i], PrimeNumberSelectedNotified));
                           }
                       }

                       minRange = DisplayablePrimeNumbers[0].Number;
                       NotifyPropertyChanged("MinRange");
                       range = DisplayablePrimeNumbers.Last().Number;
                       NotifyPropertyChanged("Range");
                   });
            }
        }

        private async Task<bool> StartPrimeNumbersComputation()
        {
            return await Task.Run(async () =>
            {
                if (PrimeM.PrimeNumbers.Count > 0 && Range <= PrimeM.PrimeNumbers.Last())
                {
                    await RearrangeRange();
                    return true;
                }
                else
                {
                    await PrimeM.SegmentedSieveEratosthenes(Range);
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                               () =>
                               {
                                   int startFrom = PrimeM.PrimeNumbers.Count - DisplayablePrimeNumberCount - 1;
                                   for (int k = startFrom > 0 ? startFrom : 0, x = 0; k < PrimeM.PrimeNumbers.Count; k++, x++)
                                   {
                                       if (DisplayablePrimeNumbers.Count > x)
                                       {
                                           DisplayablePrimeNumbers[x].Number = PrimeM.PrimeNumbers[k];
                                       }
                                       else
                                       {
                                           DisplayablePrimeNumbers.Add(new NaturalNumberViewModel(PrimeM.PrimeNumbers[k], PrimeNumberSelectedNotified));
                                       }

                                   }

                                   minRange = DisplayablePrimeNumbers[0].Number;
                                   NotifyPropertyChanged("MinRange");
                                   range = DisplayablePrimeNumbers.Last().Number;
                                   NotifyPropertyChanged("Range");
                               });
                    return true;
                }
            });
        }

        

        #region Commands
        private ICommand closeDetailViewCommand;
        public ICommand CloseDetailViewCommand
        {
            get => closeDetailViewCommand ?? (closeDetailViewCommand = new CommandHandler(() => IsDetailViewVisible = false, true));
        }

        private ICommand delayProcessCommand;
        public ICommand DelayProcessCommand
        {
            get => delayProcessCommand ?? (delayProcessCommand = new CommandHandler(() => DelayProcessCommandAction(), true));
        }

        public void DelayProcessCommandAction()
        {
            if (IsDelayedProcessing)
            {
                IsBusy = true;
                PrimeNumberProcess = new NotifyTaskCompletion<bool>(StartPrimeNumbersComputation(), new Action(() =>
                {
                    IsBusy = false;
                    IsDelayedProcessing = false;
                }));
            }
            else if (IsDelayedBackwardProcessing)
            {
                IsBusy = true;
                PrimeNumberProcess = new NotifyTaskCompletion<bool>(AdjustPrimeNumberRangesBackward(), new Action(() =>
                {
                    IsBusy = false;
                    IsDelayedBackwardProcessing = false;
                }));
            }
        }


        private ICommand pageSizeChangedCommand;
        public ICommand PageSizeChangedCommand
        {
            get => pageSizeChangedCommand ?? (pageSizeChangedCommand = new CommandHandler(() => PageSizeChangedCommandAction(), true));
        }

        public void PageSizeChangedCommandAction()
        {
            AdjustedBoundHeight = (int)Window.Current.Bounds.Height - 350;
            AdjustedBoundWidth = (int)Window.Current.Bounds.Width;
        }
        #endregion
    }
    
}
