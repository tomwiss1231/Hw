using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BackgroundPrimeCalcAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {        
        private CancellationTokenSource _cts;
        private readonly BackgroundWorker _backgroundWorker;
        private AutoResetEvent _autoEvent;
        private IProgress<int> _progress;
        private Task _timeoutTask;
        
        public MainWindow()
        {
            InitializeComponent();
            _backgroundWorker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;                        
        }

        private void BackgroundWorkerOnProgressChanged(int i)
        {
            PrimeProgress.Text = i.ToString();
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                var results = e.Result as List<int>;
                if(results == null) return;
                NumList.ItemsSource = results;
            }                        

            

        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var twoNums = e.Argument as Tuple<int, int>;
            var results = new List<int>();
            if (worker == null || twoNums == null) return;

            //_progress.Report(0);

            for (var num = twoNums.Item1; (num <= twoNums.Item2) && !worker.CancellationPending; ++num)
            {
                var isPrime = true;
                if (num == 1) isPrime = false;
                else if (num == 2) isPrime = true;
                else if (num % 2 == 0) isPrime = false;


                var boundary = (int) Math.Floor(Math.Sqrt(num));

                for (var i = 3; i <= boundary && isPrime && !worker.CancellationPending; ++i)
                {
                    if (num % i != 0) continue;
                    isPrime = false;
                    break;
                }

                if (isPrime) results.Add(num);
               // _progress.Report(Convert.ToInt32((num / (float)twoNums.Item2) * 100.0f));
                //worker.ReportProgress(Convert.ToInt32((num / (float)twoNums.Item2) * 100.0f));
            }

            e.Result = results;
            if (worker.CancellationPending)
            {
                _autoEvent.Set();
                e.Cancel = true;
            }
            
        }
        

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {            
            _cts?.Cancel();
            _autoEvent?.WaitOne();
            MessageBox.Show("Canceled", "Info");
        }

        private void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            _autoEvent = new AutoResetEvent(false);
            
            _cts = new CancellationTokenSource();
            _cts.Token.Register(_backgroundWorker.CancelAsync);
            _progress = new Progress<int>(BackgroundWorkerOnProgressChanged);
            
            var first = Convert.ToInt32(First.Text);
            var last = Convert.ToInt32(Last.Text);
            _backgroundWorker.RunWorkerAsync(new Tuple<int, int>(first, last));
            _timeoutTask = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), _cts.Token);
                    if (_backgroundWorker.IsBusy)
                    {
                        _cts.Cancel();
                    }
                }
                catch (OperationCanceledException ex)
                {                    
                    
                }

            }, _cts.Token);
        }
    }
}