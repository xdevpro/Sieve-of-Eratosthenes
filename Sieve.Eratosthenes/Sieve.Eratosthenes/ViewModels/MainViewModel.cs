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
        bool IsDelayedProcessing { get; set; }

        private bool isDelayedHeightWidthAdjustment;
        public bool IsDelayedHeightWidthAdjustment
        {
            get => isDelayedHeightWidthAdjustment;
            set
            {
                isDelayedHeightWidthAdjustment = value;
                NotifyPropertyChanged();
            }
        }

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
                IsDelayedHeightWidthAdjustment = true;
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
                IsDelayedHeightWidthAdjustment = true;
                NotifyPropertyChanged();
            }
        }

        public int DisplayablePrimeNumberCount
        {
            get => (int)((Math.Floor(AdjustedBoundHeight) / 90) * (Math.Floor(AdjustedBoundWidth) / 53));
        }
        
        private int maxRange = 10000;
        public int MaxRange
        {
            get => maxRange;
            set
            {
                if (range > value)
                {
                    Range = value;
                }
                maxRange = value;
                NotifyPropertyChanged();
                IsDelayedProcessing = true;
            }
        }
        
        private int minRange = 1;
        public int MinRange
        {
            get => minRange;
            set
            {
                int delta = value - minRange;
                minRange = value;
                if (range + delta > MaxRange)
                {
                    range = MaxRange;
                }
                else
                {
                    range += delta;
                }
                IsDelayedProcessing = true;
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
                int delta = value - range;
                range = value;
                if (minRange + delta < 1)
                {
                    minRange = 1;
                }
                else
                {
                    minRange += delta;
                }
                IsDelayedProcessing = true;
                NotifyPropertyChanged("MinRange");
                NotifyPropertyChanged();
            }
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

        void SelectPrimeNumberRange()
        {
            minRange = DisplayablePrimeNumbers[0].Number;
            NotifyPropertyChanged("MinRange");
            range = DisplayablePrimeNumbers.Last().Number;
            NotifyPropertyChanged("Range");
        }

        #endregion

        private async Task<bool> AdjustWindowHeightWidth()
        {
            return await Task.Run(async () =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                           () =>
                           {
                               while (DisplayablePrimeNumberCount < DisplayablePrimeNumbers.Count && DisplayablePrimeNumbers.Count > 3)
                               {
                                   DisplayablePrimeNumbers.RemoveAt(DisplayablePrimeNumbers.Count - 1);
                               }
                               SelectPrimeNumberRange();
                           });
                return true;
            });
        }

        private async Task<bool> StartPrimeNumbersComputation()
        {
            return await Task.Run(async () =>
            {
                if (PrimeM.PrimeNumbers.Count > 0 && Range <= PrimeM.PrimeNumbers.Last())
                {
                    int nextPrimeNumber;
                    int i;
                    if (Range == MaxRange)
                    {
                        nextPrimeNumber = Range;
                        while (!PrimeM.PrimeNumbers.Contains(--nextPrimeNumber)) ;

                        i = PrimeM.PrimeNumbers.IndexOf(nextPrimeNumber) - DisplayablePrimeNumberCount;
                    }
                    else
                    {
                        nextPrimeNumber = minRange;
                        while (!PrimeM.PrimeNumbers.Contains(++nextPrimeNumber)) ;

                        i = PrimeM.PrimeNumbers.IndexOf(nextPrimeNumber);
                    }
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
                               SelectPrimeNumberRange();
                           });
                    }
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
                                   SelectPrimeNumberRange();
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
                    IsDelayedHeightWidthAdjustment = false;
                }));
            }
            if (IsDelayedHeightWidthAdjustment)
            {
                IsBusy = true;
                PrimeNumberProcess = new NotifyTaskCompletion<bool>(AdjustWindowHeightWidth(), new Action(() =>
                {
                    IsBusy = false;
                    IsDelayedHeightWidthAdjustment = false;
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
            AdjustedBoundHeight = (int)Window.Current.Bounds.Height;
            AdjustedBoundWidth = (int)Window.Current.Bounds.Width;
        }
        #endregion
    }
    
}
