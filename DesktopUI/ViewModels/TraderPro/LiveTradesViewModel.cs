﻿using Caliburn.Micro;
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
using DesktopUI.Library.Api;
using System.Threading;

namespace DesktopUI.ViewModels.TraderPro
{
    public class LiveTradesViewModel: Screen
    {
        WebSocket ws;
        private readonly IPolygonDataEndpoint _polygonDataEndpoint;
        private readonly IEventAggregator _events;
        private readonly IUserAccountEndpoint _userAccountEndpoint;
        private readonly ITradePortfolioEndpoint _tradePortfolioEndpoint;
        private readonly ITradeTransactionEndpoint _tradeTransactionEndpoint;
        private readonly ITradeRealizedPLEndpoint _tradeRealizedPLEndpoint;

        public List<string> Labels { get; set; }
        public ChartValues<OhlcPoint> Values { get; set; }
        public ChartValues<double> Buys { get; set; }
        public ChartValues<double> Sells { get; set; }
        public int Index { get; set; }
        List<OhlcPoint> test = new List<OhlcPoint>();


        public LiveTradesViewModel(IPolygonDataEndpoint polygonDataEndpoint, IEventAggregator events, 
            IUserAccountEndpoint userAccountEndpoint, ITradePortfolioEndpoint tradePortfolioEndpoint,
            ITradeTransactionEndpoint tradeTransactionEndpoint, ITradeRealizedPLEndpoint tradeRealizedPLEndpoint)
        {
           _polygonDataEndpoint = polygonDataEndpoint;
            _events = events;
            _userAccountEndpoint = userAccountEndpoint;
            _tradePortfolioEndpoint = tradePortfolioEndpoint;
            _tradeTransactionEndpoint = tradeTransactionEndpoint;
            _tradeRealizedPLEndpoint = tradeRealizedPLEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadAccountBalance();
            await LoadPortfolioOverview("AAPL");
            StartClock();
        }

        private async Task LoadPortfolioOverview(string ticker)
        {
            //TODO CHANGE THIS TO TRADE PRORTFOLIO ENDPOINT
            var result = await _tradePortfolioEndpoint.GetPortfolioStock(ticker);
            if(result != null)
            {
                CurrentPositionAveragePrice = result.AveragePrice;
                CurrentPositionShares = result.Shares;
            }
            else
            {
                CurrentPositionShares = 0;
                CurrentPositionAveragePrice = 0;
            }
        }

        private async Task LoadAccountBalance()
        {

            var result = await _userAccountEndpoint.LoadTradesAccountBalance();
            AccountBalance = Math.Round(result, 2);
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
                    new Thread(() => PortfolioBuy()) { IsBackground = true }.Start();
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
                    new Thread(() => PortfolioSell()) { IsBackground = true }.Start();

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
                    new Thread(() => PortfolioBuy()) { IsBackground = true }.Start();

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
                    new Thread(() => PortfolioSell()) { IsBackground = true }.Start();

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

        private decimal _currentPositionAveragePrice;

        public decimal CurrentPositionAveragePrice
        {
            get { return _currentPositionAveragePrice; }
            set 
            {
                _currentPositionAveragePrice = value;
                NotifyOfPropertyChange(() => CurrentPositionAveragePrice);
            }
        }

        private int _currentPositionShares;

        public int CurrentPositionShares
        {
            get { return _currentPositionShares; }
            set 
            {
                _currentPositionShares = value;
                NotifyOfPropertyChange(() => CurrentPositionShares);
            }
        }

        private ObservableCollection<PortfolioStockDisplayModel> _portfolioStocks;

        public ObservableCollection<PortfolioStockDisplayModel> PortfolioStocks
        {
            get { return _portfolioStocks; }
            set { _portfolioStocks = value; }
        }

        private double _cashAmount;

        public double CashAmount
        {
            get 
            {
                return Math.Round(Price * Shares);
            }
            set
            {
                _cashAmount = value;
                NotifyOfPropertyChange(() => CanBuy);
                NotifyOfPropertyChange(() => CanSell);
            }
        }

        public double PortfolioProfitLoss
        {
            get
            {
                return Math.Round(((double)CurrentPositionAveragePrice - Price) * CurrentPositionShares, 2);
            }
        }

        private decimal _accountBalance;

        public decimal AccountBalance
        {
            get { return _accountBalance; }
            set 
            {
                _accountBalance = value;
                NotifyOfPropertyChange(() => AccountBalance);
            }
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
                NotifyOfPropertyChange(() => CashAmount);
                NotifyOfPropertyChange(() => CanBuy);
                NotifyOfPropertyChange(() => CanSell);
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

        public decimal ProfitLoss
        {
            get
            { 
                return Math.Round(((decimal)Price - CurrentPositionAveragePrice) * CurrentPositionShares, 2);
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
                NotifyOfPropertyChange(() => CashAmount);
                NotifyOfPropertyChange(() => ProfitLoss);
                NotifyOfPropertyChange(() => CanBuy);
                NotifyOfPropertyChange(() => CanSell);

            }
        }

        public bool CanBuy
        {
            get
            {
                if (CashAmount < (double)AccountBalance && Shares > 0) return true;
                else return false;
            }
        }

        public bool CanSell
        {
            get
            {
                if (Shares <= CurrentPositionShares && Shares > 0) return true;
                else return false;
            }
        }

        public async Task SearchForTrades()
        {
            var (opens, highs, lows, closes, dates) = await _polygonDataEndpoint.LoadTradeData(Ticker, SelectedChartInterval, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd"));

            ChartSymbol = Ticker;

            await LoadPreviousData(opens, highs, lows, closes, dates);

            StartConnection();
        }

        public async Task PortfolioBuy()
        {
            // change account balance
            await _userAccountEndpoint.UpdateTradesAccountBalance((decimal)CashAmount);

            // add to transaction table
            var transaction = new TransactionModel
            {
                Ticker = ChartSymbol,
                Buy = true,
                Price =(decimal)Price,
                Sell = false,
                Shares = Shares,
                Date = DateTime.Now
            };

            //post transaction
            await _tradeTransactionEndpoint.PostTransaction(transaction);

            //update portfolio table
            var stock = new PortfolioModel
            {
                Ticker = Ticker,
                Price = (decimal)Price,
                Shares = Shares
            };

            if (CurrentPositionShares == 0)
            {
                await _tradePortfolioEndpoint.PostStock(stock);
            }
            else
            {
                await _tradePortfolioEndpoint.UpdatePortfolioBuy(stock);
            }

            await ResetBuyPanel();
            await LoadAccountBalance();

        }

        public async Task PortfolioSell()
        {
            // add transaction to table
            var transaction = new TransactionModel
            {
                Ticker = ChartSymbol,
                Buy = false,
                Price = (decimal)Price,
                Sell = true,
                Shares = Shares,
                Date = DateTime.Now
            };
            await _tradeTransactionEndpoint.PostTransaction(transaction);

            //Update Portfolio
            var stock = new PortfolioModel
            {
                Ticker = ChartSymbol,
                Price = (decimal)Price,
                Shares = Shares
            };


            // if sold shares equals shares in portfolio delete entry
            decimal realizedProfitLoss;

            if (CurrentPositionShares == Shares)
            {
                realizedProfitLoss = await _tradePortfolioEndpoint.UpdateAndDeletePortfolio(stock);
            }
            else
            {
                // else update exisitng position
                realizedProfitLoss = await _tradePortfolioEndpoint.UpdatePortfolioSell(stock);
            }

            // update user account table, account balance and realizedgains
            var result = await _userAccountEndpoint.UpdateTradesAfterSale(realizedProfitLoss, (decimal)CashAmount);

            AccountBalance = Math.Round(result, 2);

            // update profit loss table
            await _tradeRealizedPLEndpoint.PostProfitLoss(realizedProfitLoss);

            await ResetBuyPanel();
            await LoadAccountBalance();
        }

        public void Buy()
        {
            BuyShares = true;
        }

        public void Sell()
        {
            SellShares = true;
        }

        private async Task ResetBuyPanel()
        {
            Shares = 0;
            CashAmount = 0;
            await LoadPortfolioOverview(ChartSymbol);
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
