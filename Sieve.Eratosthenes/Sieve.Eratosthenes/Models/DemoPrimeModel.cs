using Sieve.Eratosthenes.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sieve.Eratosthenes.Models
{
    public class DemoPrimeModel
    {
        List<int> DemoPrimeNumbers { get; set; }

        public async Task<bool> StraightSieveEratosthenesDemo(int limit, ObservableCollection<NaturalNumberViewModel> numbers, CancellationTokenSource taskCts)
        {
            return await Task<bool>.Run(async () =>
            {
                DemoPrimeNumbers = new List<int>();
                int sequence = 1;
                BitArray primeCandidates = new BitArray(limit + 1);
                int p, j;
                for (p = 2; p * p <= limit; p++)
                {
                    if (taskCts.IsCancellationRequested)
                        return false;
                    if (!primeCandidates[p])
                    {
                        if (DemoPrimeNumbers.Count == 0 || p > DemoPrimeNumbers?.Last())
                        {
                            DemoPrimeNumbers.Add(p);
                        }

                        for (int i = p * 2; i <= limit; i += p)
                        {
                            if (taskCts.IsCancellationRequested)
                                return false;
                            primeCandidates[i] = true;
                            var compositeNumber = numbers.FirstOrDefault(x => x.Number == i);
                            if (compositeNumber != null)
                            {
                                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                  () =>
                                  {
                                      compositeNumber.CompositeFoundSequence = sequence++;
                                  });
                                await Task.Delay(100);
                            }
                        }
                    }
                }
                return true;
            });
        }

    }
}
