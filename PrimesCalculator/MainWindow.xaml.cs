using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace PrimesCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private static List<int> CalcPrimes(int first, int last)
        {
            var results = new List<int>();
            for (var num = first; num <= last; ++num)
            {
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
            }

            return results;
        }
        
        private Thread _currentPrimeCalThread;
        private AutoResetEvent _autoEvent;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {

            _autoEvent = new AutoResetEvent(false);
            var first = Convert.ToInt32(First.Text);
            var last = Convert.ToInt32(Last.Text);
            
            
            _currentPrimeCalThread = new Thread(() =>
            {
                try
                {
                    var listOfPrime = CalcPrimes(first, last);
                    Dispatcher.Invoke(() =>
                    {
                        NumList.ItemsSource = listOfPrime;
                    });

                }
                catch (ThreadAbortException)
                {
                   _autoEvent.Set();
                }
                
                
            });                                    
            
            _currentPrimeCalThread.Start();
            
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (_currentPrimeCalThread != null && _currentPrimeCalThread.IsAlive)
            {
                _currentPrimeCalThread.Abort();
                _autoEvent.WaitOne();
                MessageBox.Show("Canceled", "Info");
            }
        }
    }
}