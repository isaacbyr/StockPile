using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DesktopUI.ViewModels.TraderPro
{
    public class PaperTradeViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly IPolygonDataEndpoint _polygonDataEndpoint;

        ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        public ChartValues<double> Buys { get; set; }
        public ChartValues<double> Sells { get; set; }
        public ChartValues<OhlcPoint> Values { get; set; }

        public PaperTradeViewModel(IStockDataEndpoint stockDataEndpoint, IPolygonDataEndpoint polygonDataEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _polygonDataEndpoint = polygonDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            StartClock();
        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("t");
        }

        private async Task DisplayChart(List<List<double>> testValues)
        {
            int index = 0;
            int count = 0;
         
            double minValue = testValues[0].Min();
            double maxValue = testValues[0].Max();

            MinAxisValue = minValue - 1;
            MaxAxisValue = maxValue + 1;
            AxisStep = 1;

            //lets instead plot elapsed milliseconds and value
            var mapper = Mappers.Xy<MeasureModel>()
                .X(x => x.ElapsedMilliseconds)
                .Y(x => x.Value);


            //save the mapper globally         
            Charting.For<MeasureModel>(mapper);
            Values = new ChartValues<OhlcPoint>();
            Buys = new ChartValues<double>();
            Sells = new ChartValues<double>();

            var transactions = new List<PaperTradeModel>();

            var sw = new Stopwatch();
            sw.Start();
            var numCandle = 0;

            IsRunning = true;
            manualResetEvent.Set();

            await Task.Run(() =>
            {

                while (numCandle < testValues.Count && IsRunning)
                {
                    //manualResetEvent.Set();
                    Thread.Sleep(SpeedOuter);

                    if (count % 2 != 0)
                    {

                        //Thread.Sleep(500);
                        var point = new OhlcPoint();
                        double high = testValues[index - 1][0];
                        double low = testValues[index - 1][0];
                        double open = testValues[index - 1][0];

                        for (int j = 1; j < testValues[index - 1].Count; j++)
                        {
                            Thread.Sleep(SpeedInner);
                            if (testValues[index - 1][j] >= testValues[index - 1][j - 1])
                            {
                                if (testValues[index - 1][j] > high)
                                {
                                    high = testValues[index - 1][j];
                                }
                                point = new OhlcPoint(open, high, low, testValues[index - 1][j]);
                            }
                            else
                            {
                                if (testValues[index - 1][j] < low)
                                {
                                    low = testValues[index - 1][j];
                                }
                                point = new OhlcPoint(open, high, low, testValues[index - 1][j]);
                            }


                            if (BuyShares == true)
                            {
                                Buys[index - 1] = open;
                                BuyShares = false;
                                var transaction = new PaperTradeModel
                                {
                                    BuyOrSell = "Buy",
                                    Price = open,
                                    Shares = Shares
                                };
                                transactions.Add(transaction);
                            }
                            else
                            {
                                if (double.IsNaN(Buys[index-1]) && double.IsInfinity(Buys[index-1]))
                                {
                                    Buys[index - 1] = double.NaN;
                                }
                            }

                            if(SellShares == true)
                            {
                                Sells[index - 1] = open;
                                SellShares = false;
                                var transaction = new PaperTradeModel
                                {
                                    BuyOrSell = "Sell",
                                    Price = open,
                                    Shares = Shares
                                };
                                transactions.Add(transaction);
                            }
                            else
                            {
                                if(double.IsNaN(Sells[index-1]) && double.IsInfinity(Sells[index-1]))
                                {
                                    Sells[index - 1] = double.NaN;
                                }
                            }

                            Values[index - 1] = point;

                            NotifyOfPropertyChange(() => Values);
                            NotifyOfPropertyChange(() => Buys);
                            NotifyOfPropertyChange(() => Sells);

                        }
                        count++;
                        numCandle++;
                    }
                    else
                    {

                        //we add the lecture based on our StopWatch instance
                        var point = new OhlcPoint(testValues[index][0], testValues[index][0], testValues[index][0], testValues[index][0]);
                        Values.Insert(index, point);

                        if (BuyShares == true)
                        {
                            Buys.Insert(index, testValues[index][0]);
                            BuyShares = false;
                            var transaction = new PaperTradeModel
                            {
                                BuyOrSell = "Buy",
                                Price = testValues[index][0],
                                Shares = Shares
                            };
                            transactions.Add(transaction);
                        }
                        else
                        {
                            Buys.Insert(index, double.NaN);
                        }

                        if(SellShares == true)
                        {
                            Sells.Insert(index, testValues[index][0]);
                            SellShares = false;
                            var transaction = new PaperTradeModel
                            {
                                BuyOrSell = "Sell",
                                Price = testValues[index][0],
                                Shares = Shares
                            };
                            transactions.Add(transaction);
                        }
                        else
                        {
                            Sells.Insert(index, double.NaN);
                        }

                        index++;
                        count++;
                        NotifyOfPropertyChange(() => Values);
                        NotifyOfPropertyChange(() => Buys);
                        NotifyOfPropertyChange(() => Sells);
                    }


                    //lets only use the last 150 values
                    if (Values.Count > 150)
                    {
                        Values.RemoveAt(0);
                    }
                }
            });

            IsRunning = false;

            Transactions = new BindingList<PaperTradeModel>(transactions);
            NotifyOfPropertyChange(() => Transactions);
        }

        private string _currentTime = DateTime.Now.ToString("t");

        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                NotifyOfPropertyChange(() => CurrentTime);
            }
        }


        private string _ticker;

        public string Ticker
        {
            get { return _ticker; }
            set 
            {
                _ticker = value;
                NotifyOfPropertyChange(() => Ticker);
            }
        }

        private List<int> _chartInterval = new List<int> { 5, 10, 15 };

        public List<int> ChartInterval
        {
            get { return _chartInterval; }
            set { _chartInterval = value; }
        }

        private int _selectedChertInterval;

        public int SelectedChartInterval
        {
            get { return _selectedChertInterval; }
            set 
            {
                _selectedChertInterval = value;
                NotifyOfPropertyChange(() => SelectedChartInterval);
            }
        }


        private DateTime _selectedDate = DateTime.Now;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set 
            { 
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
            }
        }

        private double _axisStep;

        public double AxisStep
        {
            get { return _axisStep; }
            set
            {
                _axisStep = value;
                NotifyOfPropertyChange(() => AxisStep);
            }
        }


        private double _axisUnit;

        public double AxisUnit
        {
            get { return _axisUnit; }
            set
            {
                _axisUnit = value;
                NotifyOfPropertyChange(() => AxisUnit);
            }
        }


        public void SetAxisLimits(DateTime now)
        {
            XAxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            XAxisMin = now.Ticks - TimeSpan.FromSeconds(20).Ticks;

        }

        private BindingList<OhlcStockModel> _stock;

        public BindingList<OhlcStockModel> Stock
        {
            get { return _stock; }
            set 
            {
                _stock = value;
                NotifyOfPropertyChange(() => Stock);
            }
        }

        private double _minAxisValue = 0;

        public double MinAxisValue
        {
            get { return _minAxisValue; }
            set 
            {
                _minAxisValue = value;
                NotifyOfPropertyChange(() => MinAxisValue);
            }
        }

        private double _maxAxisValue = 100;

        public double MaxAxisValue
        {
            get { return _maxAxisValue; }
            set 
            {
                _maxAxisValue = value;
                NotifyOfPropertyChange(() => MaxAxisValue);
            }
        }

        private double _xAxisMax;

        public double XAxisMax
        {
            get { return _xAxisMax; }
            set
            {
                _xAxisMax = value;
                NotifyOfPropertyChange(() => XAxisMax);
            }
        }

        private double _xAxisMin = 0;

        public double XAxisMin
        {
            get { return _xAxisMin; }
            set
            {
                _xAxisMin = value;
                NotifyOfPropertyChange(() => XAxisMin);
            }
        }

        private bool _isRunning = false;

        public bool IsRunning
        {
            get { return _isRunning; }
            set 
            { 
                _isRunning = value;
                NotifyOfPropertyChange(() => IsRunning);
            }
        }

        private int _speedOuter = 1000;

        public int SpeedOuter
        {
            get { return _speedOuter; }
            set 
            {
                _speedOuter = value;
                NotifyOfPropertyChange(() => SpeedOuter);
            }
        }

        private int _speedInner = 100;

        public int SpeedInner
        {
            get { return _speedInner; }
            set
            {
                _speedInner = value;
                NotifyOfPropertyChange(() => SpeedInner);
            }
        }

        private bool _buyShares;

        public bool BuyShares
        {
            get { return _buyShares; }
            set 
            {
                _buyShares = value;
                NotifyOfPropertyChange(() => BuyShares);
            }
        }

        private bool _sellShares = false;

        public bool SellShares
        {
            get { return _sellShares; }
            set 
            {
                _sellShares = value;
                NotifyOfPropertyChange(() => SellShares);
            }
        }

        private int _shares;

        public int Shares
        {
            get { return _shares; }
            set 
            { 
                _shares = value;
                NotifyOfPropertyChange(() => Shares);
            }
        }


        private BindingList<PaperTradeModel> _transactions;

        public BindingList<PaperTradeModel> Transactions
        {
            get { return _transactions; }
            set 
            {
                _transactions = value;
                NotifyOfPropertyChange(() => Transactions);
            }
        }

        private double _profitLoss;

        public double ProfitLoss
        {
            get { return _profitLoss; }
            set 
            {
                _profitLoss = value;
                NotifyOfPropertyChange(() => ProfitLoss);
            }
        }


        public async Task SearchForTrades()
        {
            var (open, high, low, close, dates) = await _polygonDataEndpoint.LoadTradeData(Ticker, SelectedChartInterval, SelectedDate.ToString("yyyy-MM-dd"));

            var trades =  await GroupStockResults(open, high, low, close, SelectedChartInterval);
            await DisplayChart(trades);

            await GetProfitLoss();
        }

        private async Task<List<List<double>>> GroupStockResults(List<double> open, List<double> high, List<double> low, List<double> close, int interval)
        {
            int index = 0;
            var Trades = new List<List<double>>();

            var intervalList = new List<double>();
            while(index < open.Count)
            {
                for(int i = index; i < index + interval; i++)
                {
                    intervalList.Add(open[i]);
                    intervalList.Add(high[i]);
                    intervalList.Add(low[i]);
                    intervalList.Add(close[i]);
                }
                Trades.Add(intervalList);
                intervalList = new List<double>();

                index += interval; 
            }

            return Trades;
        }

        public void Start()
        {
            if(IsRunning == true)
            {
                return;
            }
            else
            {
                IsRunning = true;
                manualResetEvent.Set();
            }
        }

        public void Pause()
        {
            if(IsRunning == true)
            {
                IsRunning = false;
                manualResetEvent.Reset();
            }
            else
            {
                return;
            }
        }

        public void SpeedUp()
        {
            if(IsRunning == true)
            {
                if (SpeedOuter > 1 && SpeedInner > 1)
                {
                    SpeedOuter /= 2;
                    SpeedInner /= 2;
                }
                else
                {
                    return;
                }
                
            }
            else
            {
                return;
            }
        }

        public void SlowDown()
        {
            if (IsRunning == true)
            {
                if(SpeedOuter < 5000 & SpeedInner < 500 )
                {
                    SpeedOuter += 500;
                    SpeedInner += 50;
                    
                }
                else
                {
                    return;
                }
                
            }
            else
            {
                return;
            }
        }

        public void Buy()
        {
            BuyShares = true;
        }

        public void Sell()
        {
            SellShares = true;
        }

        private async Task GetProfitLoss()
        {

            if (Transactions.Count <= 1)
            {
                ProfitLoss = 0;
                return;
            }

            double sum = 0;
            double avgPrice = 0;
            int shares = 0;

            if (Transactions[0].BuyOrSell == "Buy")
            {
                avgPrice = Transactions[0].Price;
                shares = Transactions[0].Shares;

                for (int i = 1; i < Transactions.Count; i++)
                {
                    if(Transactions[i].BuyOrSell == "Buy")
                    {
                        avgPrice = ((avgPrice * shares) + (Transactions[i].Price * Transactions[i].Shares)) / (shares + Transactions[i].Shares);
                        shares = shares + Transactions[i].Shares;
                    }
                    else
                    {
                        sum += (Transactions[i].Price - avgPrice) * Transactions[i].Shares;
                        shares -= Transactions[i].Shares;
                    }
                }
            }
            else
            {
                avgPrice = Transactions[0].Price;
                shares = Transactions[0].Shares;

                for (int i = 1; i < Transactions.Count; i++)
                {
                    if (Transactions[i].BuyOrSell == "Sell")
                    {
                        avgPrice = ((avgPrice * shares) + (Transactions[i].Price * Transactions[i].Shares)) / (shares + Transactions[i].Shares);
                        shares = shares + Transactions[i].Shares;
                    }
                    else
                    {
                        sum += (Transactions[i].Price - avgPrice) * Transactions[i].Shares;
                        shares -= Transactions[i].Shares;
                    }
                }
            }

            ProfitLoss = Math.Round(sum, 2);
        }

    }

    
}
