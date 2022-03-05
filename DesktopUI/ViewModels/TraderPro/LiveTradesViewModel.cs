using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocket4Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DesktopUI.Library.Models;
using LiveCharts;
using LiveCharts.Defaults;
using DesktopUI.Library.Api.TraderPro;
using System.ComponentModel;
using DesktopUI.Library.Models.TraderPro;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using DesktopUI.Library.EventModels.TraderPro;
using DesktopUI.Library.EventModels;

namespace DesktopUI.ViewModels.TraderPro
{
    public class LiveTradesViewModel: Screen
    {
        WebSocket ws;
        private readonly IPolygonDataEndpoint _polygonDataEndpoint;
        private readonly IEventAggregator _events;

        public List<string> Labels { get; set; }
        public ChartValues<OhlcPoint> Values { get; set; }
        public ChartValues<double> Buys { get; set; }
        public ChartValues<double> Sells { get; set; }
        public int Index { get; set; }
        List<OhlcPoint> test = new List<OhlcPoint>();


        public LiveTradesViewModel(IPolygonDataEndpoint polygonDataEndpoint, IEventAggregator events)
        {
           _polygonDataEndpoint = polygonDataEndpoint;
            _events = events;
        }

        protected override void OnViewLoaded(object view)
        {
            StartClock();
        }

        private void StartConnection()
        {
            ws = new WebSocket("wss://socket.polygon.io/stocks", sslProtocols: SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
            ws.Opened += ws_Opened;
            ws.Error += ws_Error;
            ws.Closed += ws_Closed;
            ws.MessageReceived += ws_MessageRecieved;
            ws.Open();

        }

        private void ws_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Closed...");
        }

        private void ws_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket Error");
            Console.WriteLine(e.Exception.Message);
        }

        private void ws_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connected!");
            ws.Send("{\"action\":\"auth\",\"params\":\"g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1\"}");
            ws.Send("{\"action\":\"subscribe\",\"params\":\"A.AAPL\"}");
        }

        private void ws_MessageRecieved(object sender, MessageReceivedEventArgs e)
        {

            var data = JArray.Parse(e.Message);

            if (IsConnecting)
            {
                foreach (JObject parsedObject in data.Children<JObject>())
                {
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        //string propertyName = parsedProperty.Name;
                        string propertyValue = (string)parsedProperty.Value;

                        if (propertyValue.Equals("success") && Count == 2)
                        {
                            IsConnecting = false;
                        }
                    }
                }
            }
            else
            {
                ConvertAndChart(data);
            }

            Count += 1;
        }

        private async Task LoadPreviousData(List<double> opens, List<double> highs, List<double> lows, List<double> closes, List<DateTime> dates)
        {
            Buys = new ChartValues<double>();
            Sells = new ChartValues<double>();
            Transactions = new AsyncObservableCollection<PaperTradeModel>();
            Labels = new List<string>();
            Values = new ChartValues<OhlcPoint>();
            Index = 0;

            YMaxAxis = highs.Max() + 1;
            YMinAxis = lows.Min() - 1;
            XAxisMax = opens.Count + 10;

            for(int i = 0; i < opens.Count; i++)
            {
                var ohlc = new OhlcPoint(opens[i], highs[i], lows[i] ,closes[i]);
                Values.Add(ohlc);
                test.Add(ohlc);
                Labels.Add(dates[i].ToString("t"));

                Sells.Add(double.NaN);
                Buys.Add(double.NaN);

                Index++;
            }

            IntervalHigh = Values[Index - 1].Close;
            IntervalLow = Values[Index - 1].Close;
            Price = Values[Index - 1].Close;

            NotifyOfPropertyChange(() => Values);
            NotifyOfPropertyChange(() => Labels);
            NotifyOfPropertyChange(() => Buys);
            NotifyOfPropertyChange(() => Sells);
            NotifyOfPropertyChange(() => XAxisMax);
        }

        private void ConvertAndChart(JArray data)
        {
            XAxisMax = Values.Count + 10;

            double open = Values[Index - 1].Close;
            double high;
            double low;
            double close = Values[Index - 1].Close;
            DateTime dt = DateTime.Now;


            foreach (JObject parsedObject in data.Children<JObject>())
            {
                foreach (JProperty parsedProperty in parsedObject.Properties())
                {
                    string propertyName = parsedProperty.Name;

                    if (propertyName.Equals("o"))
                    {
                        open = (double)parsedProperty.Value;
                    }
                    else if (propertyName.Equals("h"))
                    {
                        high = (double)parsedProperty.Value;
                        if(high > IntervalHigh)
                        {
                            IntervalHigh = high;
                        }
                    }
                    else if (propertyName.Equals("l"))
                    {
                        low = (double)parsedProperty.Value ;
                        if(low < IntervalLow)
                        {
                            IntervalLow = low;
                        }
                    }
                    else if (propertyName.Equals("c"))
                    {
                        close = (double)parsedProperty.Value;
                        Price = Math.Round(close, 2);
                    }
                    else if (propertyName.Equals("s"))
                    {
                        var ts = (long)parsedProperty.Value;
                        dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(ts).ToLocalTime();
                    }
                }
            }

            if (dt.Minute % SelectedChartInterval == 0 && dt.Second == 0)
            {
                var ohlc = new OhlcPoint(open, IntervalHigh, IntervalLow, close);

                Values.Add(ohlc);
                test.Add(ohlc);
                Labels.Add(dt.ToString("t"));

                IntervalHigh = close;
                IntervalLow = close;

                if (BuyShares == true)
                {
                    Buys.Insert(Index, open);
                    BuyShares = false;
                    var transaction = new PaperTradeModel
                    {
                        BuyOrSell = "Buy",
                        Price = close,
                        Shares = Shares
                    };
                    Transactions.Add(transaction);
                }
                else
                {
                    Buys.Insert(Index, double.NaN);
                }

                if (SellShares == true)
                {
                    Sells.Insert(Index, open);
                    SellShares = false;
                    var transaction = new PaperTradeModel
                    {
                        BuyOrSell = "Sell",
                        Price = open,
                        Shares = Shares
                    };
                    Transactions.Add(transaction);
                }
                else
                {
                    Sells.Insert(Index, double.NaN);
                }

                Index++;
            }
            else
            {
                var ohlc = new OhlcPoint(Values[Index-2].Close, IntervalHigh, IntervalLow, close);

                Values[Index-1] = ohlc;
                test[Index-1] = ohlc;

                if (BuyShares == true)
                {
                    Buys[Index - 1] = open;
                    BuyShares = false;
                    var transaction = new PaperTradeModel
                    {
                        BuyOrSell = "Buy",
                        Price = open,
                        Shares = Shares
                    };
                    Transactions.Add(transaction);
                }
                else
                {
                    if (double.IsNaN(Buys[Index - 1]) && double.IsInfinity(Buys[Index - 1]))
                    {
                        Buys[Index - 1] = double.NaN;
                    }
                }

                if (SellShares == true)
                {
                    Sells[Index - 1] = open;
                    SellShares = false;
                    var transaction = new PaperTradeModel
                    {
                        BuyOrSell = "Sell",
                        Price = open,
                        Shares = Shares
                    };
                    Transactions.Add(transaction);
                }
                else
                {
                    if (double.IsNaN(Sells[Index - 1]) && double.IsInfinity(Sells[Index - 1]))
                    {
                        Sells[Index - 1] = double.NaN;
                    }
                }
            }

            NotifyOfPropertyChange(() => Values);
            NotifyOfPropertyChange(() => Labels);
            NotifyOfPropertyChange(() => Buys);
            NotifyOfPropertyChange(() => Sells);
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

        private bool _isConnecting = true;

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set 
            {
                _isConnecting = value;
                NotifyOfPropertyChange(() => IsConnecting);
            }
        }

        private int _count = 0;

        public int Count
        {
            get { return _count; }
            set 
            {
                _count = value;
                NotifyOfPropertyChange(() => Count);
            }
        }

        private double _yminAxis = 0;

        public double YMinAxis
        {
            get { return _yminAxis; }
            set
            {
                _yminAxis = value;
                NotifyOfPropertyChange(() => YMinAxis);
            }
        }

        private double _ymaxAxis = 100;

        public double YMaxAxis
        {
            get 
            {
                return _ymaxAxis;
            }
            set
            {
                _ymaxAxis = value;
                NotifyOfPropertyChange(() => YMaxAxis);
            }
        }

        private double _xAxisMax = 50;

        public double XAxisMax
        {
            get 
            {
                return _xAxisMax;
            }
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

        private ObservableCollection<PaperTradeModel> _transactions;

        public ObservableCollection<PaperTradeModel> Transactions
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

        private List<int> _chartInterval = new List<int> { 5, 10, 15 };

        public List<int> ChartInterval
        {
            get { return _chartInterval; }
            set { _chartInterval = value; }
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

        private double _intervalHigh;

        public double IntervalHigh
        {
            get { return _intervalHigh; }
            set 
            {
                _intervalHigh = value;
                NotifyOfPropertyChange(() => IntervalHigh);
            }
        }

        private double _intervalLow;

        public double IntervalLow
        {
            get { return _intervalLow; }
            set 
            {
                _intervalLow = value;
                NotifyOfPropertyChange(() => IntervalLow);
            }
        }

        private string _chartSymbol;

        public string ChartSymbol
        {
            get { return _chartSymbol; }
            set 
            {
                _chartSymbol = value;
                NotifyOfPropertyChange(() => ChartSymbol);
            }
        }

        private double _price;

        public double Price
        {
            get { return _price; }
            set 
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }



        public async Task SearchForTrades()
        {
            var (opens, highs, lows, closes, dates) = await _polygonDataEndpoint.LoadTradeData(Ticker, SelectedChartInterval, DateTime.Now.ToString("yyyy-MM-dd"));

            ChartSymbol = Ticker;

            await LoadPreviousData(opens, highs, lows, closes, dates);

            StartConnection();
        }


        public void Buy()
        {
            BuyShares = true;
        }

        public void Sell()
        {
            SellShares = true;
        }

        public void TradeCrossovers()
        {
            _events.PublishOnUIThread(new LaunchTraderProEvent());
        }

        public void OpenStrategies()
        {
            _events.PublishOnUIThread(new OpenStrategiesView());
        }

        public void PaperTrade()
        {
            _events.PublishOnUIThread(new OpenPaperTradeView());
        }

        public void Menu()
        {
            _events.PublishOnUIThread(new OpenMainMenuEvent());
        }
    }
}
