using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Primes5._3
{
    internal static class Program
    {
        private static List<int> CalcPrimes(int first, int last)
        {
            
            
            var results = new ConcurrentBag<int>();
            
            var random = new Random();

            var cts = new CancellationTokenSource();

            var option = new ParallelOptions
            {
                CancellationToken = cts.Token,                
            };
            try
            {
                Parallel.For(first, last + 1, option,(num) =>
                {
                    var randomInt = random.Next(10000000);
                    if (randomInt == 0)
                    {                                        
                        cts.Cancel();
                    }                                
                
                    var isPrime = true; 
                    if (num == 1) isPrime = false;
                    else if (num == 2) isPrime =  true;
                    else if (num % 2 == 0)  isPrime = false;
                
                
                    var boundary = (int)Math.Floor(Math.Sqrt(num));
                
                    for (var i = 3; i <= boundary && isPrime; ++i)
                    {
                        if (num % i != 0) continue;
                        isPrime = false;
                        break;
                    }

                    if (isPrime) results.Add(num);                               
                });

            }
            catch (OperationCanceledException)
            {
            } 


            return results.OrderBy(num => num).ToList();
        }

        public static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var listToDisplay = CalcPrimes(1, 30000000);
            
            stopWatch.Stop();
            
            var listToDisplayStr = "[" + string.Join(", ", listToDisplay) + "]";

            Console.WriteLine($" The primes are: {listToDisplayStr} elapsed: {stopWatch.Elapsed.TotalMilliseconds}");
        }
    }
}