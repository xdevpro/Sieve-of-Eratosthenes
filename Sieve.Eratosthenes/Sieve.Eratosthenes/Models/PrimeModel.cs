using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieve.Eratosthenes.Models
{
    public class PrimeModel
    {
        public List<int> PrimeNumbers { get; set; }

        public PrimeModel()
        {
            PrimeNumbers = new List<int>();
        }

        async Task<bool> StraightSieveEratosthenes(int limit)
        {
            return await Task.Run(() =>
            {
                BitArray primeCandidates = new BitArray(limit + 1);
                int p, j;
                for (p = 2; p * p <= limit; p++)
                {
                    if (!primeCandidates[p])
                    {
                        if (PrimeNumbers.Count == 0 || p > PrimeNumbers?.Last())
                        {
                            PrimeNumbers.Add(p);
                        }
                        for (int i = p * 2; i <= limit; i += p)
                        {
                            primeCandidates[i] = true;
                        }
                    }
                }

                for (j = p; j <= limit; j++)
                {
                    if (!primeCandidates[j] && PrimeNumbers.Count > 0 && j > PrimeNumbers?.Last())
                    {
                        PrimeNumbers.Add(j);
                    }
                }
                return true;
            }
            );
        }

        public async Task<bool> SegmentedSieveEratosthenes(int range)
        {
            // Compute all primes smaller than or equal to square root of MaxRange using simple sieve
            int limit = (int)Math.Floor(Math.Sqrt(range)) + 1;
            await StraightSieveEratosthenes(limit);

            return await Task.Run(() =>
            {
                var primes = PrimeNumbers.TakeWhile(p => p < limit).ToList();
                //Divide the range [0.....MaxRange-1] in different segments
                //Chosen segment size as Math.Sqrt(MaxRange)
                int low = limit;
                int high = 2 * limit;

                //While all segments of range [0....MaxRange-1] are not processed, process one segment at a time
                while (low < range)
                {
                    BitArray primeCandidates = new BitArray(limit + 1);
                    for (int i = 0; i < primes.Count(); i++)
                    {
                        //Find the minimum number in [low..high] that is
                        //a multiple of prime[i] (divisible by prime[i])
                        //For example, if low is 31 and prime[i] is 3, 
                        //start with 33.
                        double a = low / primes[i];
                        int loLim = (int)(Math.Floor(a) * primes[i]);

                        if (loLim < low)
                            loLim += primes[i];

                        /* Mark multiples of prime[i] in [low..high]:
                           We are marking [j - low] for j, i.e. each number
                           in range [low, high] is mapped to [0, high-low]
                           so if range is [50, 100] marking 50 corresponds
                           to marking 0, marking 51 corresponds to 1 and so on.
                           In this way we need to allocate space only for range */
                        for (int j = loLim; j < high; j += primes[i])
                        {
                            primeCandidates[j - low] = true;
                        }
                    }

                    //Numbers which are not marked as true are prime
                    for (int i = low; i < high; i++)
                    {
                        if (!primeCandidates[i - low] && PrimeNumbers.Count > 0 && i > PrimeNumbers?.Last())
                        {
                            PrimeNumbers.Add(i);
                        }
                    }

                    // Update low and high for next segment
                    low = low + limit;
                    high = high + limit;
                    if (high >= range) high = range;
                }
                return true;
            });
        }
    }
}
