using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.EventModels.TraderPro;
using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DesktopUI.ViewModels.TraderPro
{
    public class TraderMainViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly IStrategyEndpoint _strategyEndpoint;
        private readonly IWindowManager _window;
        private readonly TransactionInfoViewModel _transactionInfoVM;
        private readonly ExistingStrategiesViewModel _existingStrategiesVM;
        private readonly IEventAggregator _events;

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        public int ChartLength { get; set; }

        public List<List<decimal>> IndicatorList { get; set; } = new List<List<decimal>>();

        public TraderMainViewModel(IStockDataEndpoint stockDataEndpoint, IStrategyEndpoint strategyEndpoint,
            IWindowManager window, TransactionInfoViewModel transactionInfoVM, 
            ExistingStrategiesViewModel existingStrategiesVM, IEventAggregator events)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _strategyEndpoint = strategyEndpoint;
            _window = window;
            _transactionInfoVM = transactionInfoVM;
            _existingStrategiesVM = existingStrategiesVM;
            _events = events;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadChart("AAPL");
            StartClock();
        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("t");
        }


        private async Task LoadChart(string ticker, string range = "3mo", string interval = "1d")
        {
            Labels.Clear();
            SeriesCollection = new SeriesCollection();
            var Values = new ChartValues<OhlcPoint>();
            var volume = new ChartValues<long>();
            Prices = new List<decimal>(); 

            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts(ticker, range, interval);

            ChartLength = results.Count;

            ChartPrice = marketPrice;
            ChartSymbol = symbol;

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double)result.Open, 2), Math.Round((double)result.High, 2), Math.Round((double)result.Low, 2), Math.Round((double)result.Close, 2));
                volume.Add(result.Volume);
                Values.Add(point);
                Labels.Add(result.Date);
                Prices.Add( Math.Round(result.Open ,2));
            }

            SeriesCollection.Add(
                new CandleSeries()
                {
                    Title = symbol,
                    Values = Values,
                    StrokeThickness = 4
                });


            NotifyOfPropertyChange(() => SeriesCollection);
            NotifyOfPropertyChange(() => Labels);
            NotifyOfPropertyChange(() => ChartLength);
        }

        private async Task LoadRegressionData(string color)
        {
            //convert color form string to SolidColorBrush
            Color newColor = (Color)ColorConverter.ConvertFromString(color);
            SolidColorBrush brush = new SolidColorBrush(newColor);

            var result = await _stockDataEndpoint.GetCloseData(ChartSearch, SelectedChartRange, SelectedChartInterval);

            if (result.Count > 0)
            {
                // y = a + bx
                int n = result.Count;
                List<decimal> xy = new List<decimal>();
                List<decimal> x = new List<decimal>();
                List<decimal> x2 = new List<decimal>();
                List<decimal> y = new List<decimal>();
                decimal sumx2 = 0;
                for (int i = 0; i < n; i++)
                {
                    xy.Add(result[i].Close * (i + 1));
                    x.Add(i + 1);
                    y.Add(result[i].Close);
                    x2.Add((i + 1) * (i + 1));
                }
                decimal xbar = x.Sum() / n;
                decimal ybar = y.Sum() / n;

                decimal sumxy = xy.Sum();
                sumx2 = x2.Sum();
                decimal sumx = x.Sum();
                decimal sumy = y.Sum();

                decimal b = 0;
                decimal a = 0;

                b =  (n * (sumxy - sumx * sumy) ) / (n*(sumx2 -  sumx * sumx));
                a = ybar - b * xbar;

                var Values = new ChartValues<decimal>();

                for (int i = 0; i < n; i++)
                {
                    var value = a + b * x[i];
                    Values.Add(value);
                }

                var lastValue = x[n - 1];
                Values.Add(a + b * (lastValue + 1));
                Values.Add(a + b * (lastValue + 2));
                Values.Add(a + b * (lastValue + 3));


                SeriesCollection.Add(
                    new LineSeries
                    {
                        Values = Values,
                        Title = "Regression",
                        Fill = System.Windows.Media.Brushes.Transparent,
                        Stroke = brush,
                    });
            }
        }


        public async Task LoadEMA(string emaInterval, string color)
        {
            //convert color form string to SolidColorBrush
            Color newColor = (Color)ColorConverter.ConvertFromString(color);
            SolidColorBrush brush = new SolidColorBrush(newColor);

            int emaRange;
            int.TryParse(emaInterval, out emaRange);

            var Values = new ChartValues<decimal>();

            var maList = new List<decimal>();

            if(ChartSearch != null && SelectedChartInterval != null && SelectedChartRange != null)
            {
                var (range, lastResults) = AddAndConvertDays(SelectedChartRange, SelectedChartInterval, emaRange);

                var (results, symbol, marketPrice) = await _stockDataEndpoint.GetMAChartData(ChartSearch, range, SelectedChartInterval, lastResults);
                ChartPrice = marketPrice;
                ChartSymbol = symbol;

                decimal k = (decimal)2 / (emaRange + 1);
                decimal sum = 0;
                //get sma
                for(int i = 0; i < emaRange; i++)
                {
                    sum += results[i].Close;
                }
                decimal sma = sum / emaRange;
                Values.Add(sma);
                maList.Add(sma);

                int index = 0;
                for(int i = emaRange; i < results.Count; i++)
                {
                    decimal ema = (results[i].Close * k) + (maList[index] * (1 - k));
                    Values.Add(ema);
                    maList.Add(ema);

                    index += 1;
                }

                var removeValues = Values.Count - ChartLength;
                int idx = 0;
                while (idx < removeValues)
                {
                    Values.RemoveAt(0);
                    maList.RemoveAt(0);
                    idx++;
                }

                SeriesCollection.Add(
                    new LineSeries
                    {
                        Values = Values,
                        Title = $"{emaRange}d SMA",
                        Fill = System.Windows.Media.Brushes.Transparent,
                        Stroke = brush,
                        StrokeThickness = 2,
                        PointGeometry = null,

                    });

                IndicatorList.Add(maList);
            }
            else
            {
                return;
            }
            NotifyOfPropertyChange(() => SeriesCollection);
            NotifyOfPropertyChange(() => Labels);
        }

        public async Task LoadSMA(string smaInterval,string color)
        {
            //convert color form string to SolidColorBrush
            Color newColor = (Color)ColorConverter.ConvertFromString(color);
            SolidColorBrush brush = new SolidColorBrush(newColor);
            
            int smaRange;
            int.TryParse(smaInterval, out smaRange);

            var Values = new ChartValues<decimal>();

           var maList = new List<decimal>();

            if (ChartSearch != null && SelectedChartInterval != null && SelectedChartRange != null)
            {

                var (range, lastResults) = AddAndConvertDays(SelectedChartRange, SelectedChartInterval, smaRange);

                var (results, symbol, marketPrice) = await _stockDataEndpoint.GetMAChartData(ChartSearch, range, SelectedChartInterval, lastResults);
                ChartPrice = marketPrice;
                ChartSymbol = symbol;

                int lower = 0;
                int upper = smaRange;
                while(upper <= results.Count)
                {
                    decimal sum = 0;

                    for (int i = lower; i < upper; i++)
                    {
                        sum += results[i].Close;
                    }

                    var chartVal = sum / smaRange;
                    Values.Add(Math.Round(chartVal,2));
                    maList.Add(Math.Round(chartVal, 2));

                    lower += 1;
                    upper += 1;
                }

                var removeValues = Values.Count - ChartLength;
                int index = 0;
                while(index < removeValues) 
                {
                    Values.RemoveAt(0);

                    index++;
                }

                SeriesCollection.Add(
                    new LineSeries
                    {
                        Values = Values,
                        Title = $"{smaRange}d SMA",
                        Fill = System.Windows.Media.Brushes.Transparent,
                        Stroke = brush, 
                        StrokeThickness = 2,
                        PointGeometry = null,
                        
                    }); 

                IndicatorList.Add(maList);
            }
            else
            {
                return;
            }

            NotifyOfPropertyChange(() => SeriesCollection);
            NotifyOfPropertyChange(() => Labels);
        }

        // return interval and string
        public (string, int) AddAndConvertDays(string range, string interval, int smaRange)
        {
            decimal temp;

            if (SelectedChartRange == "1d" || SelectedChartRange == "5d")
            {
                decimal days;
                decimal.TryParse(SelectedChartRange.Substring(0, 1), out days);

                var denom = Math.Ceiling(ChartLength / days);
                temp = Math.Ceiling((ChartLength + (decimal)smaRange) / denom);

                return ($"{temp}d", ChartLength + smaRange);
            }
            else if (SelectedChartRange == "1mo" && SelectedChartInterval == "5m")
            {
               
                temp = Math.Ceiling((ChartLength + (decimal)smaRange) / 79);

                return ($"{temp}d", ChartLength + smaRange);
            }
            else if (SelectedChartRange == "1mo" && SelectedChartInterval == "15m")
            {

                temp = Math.Ceiling((ChartLength + (decimal)smaRange) / 27);

                return ($"{temp}d", ChartLength + smaRange);
            }
            else if (SelectedChartRange == "1mo" && SelectedChartInterval == "1d")
            {
                temp = (ChartLength + smaRange);
                return ($"{temp}d", ChartLength + smaRange);
            }
            else if ( SelectedChartRange == "3mo" || SelectedChartRange == "6mo")
            {
                temp = (ChartLength + smaRange);
                return ($"{temp}d", ChartLength + smaRange);

            }
            //TODO SET THIS UP PROPERLY FOR 1YR AND 5YR
            return ("", 0);
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



        private BindingList<string> _indicatorInterval = new BindingList<string> { "4", "9", "13", "21", "50" };

        public BindingList<string> IndicatorInterval
        {
            get
            {
                if (SelectedIndicator == "EMA" || SelectedIndicator == "SMA")
                {
                    return _indicatorInterval = new BindingList<string> { "4", "9", "13", "21", "50" };
                }
                else
                {
                    return _indicatorInterval = new BindingList<string>();
                }
            }
            set 
            {
                _indicatorInterval = value;
                NotifyOfPropertyChange(() => IndicatorInterval);
            }
        }

        private string _selectedIndicatorInterval;

        public string SelectedIndicatorInterval
        {
            get { return _selectedIndicatorInterval; }
            set 
            {
                _selectedIndicatorInterval = value;
                NotifyOfPropertyChange(() => SelectedIndicatorInterval);
            }
        }

        private List<string> _colors = new List<string> { "Turquoise" , "Gold", "Fuchsia", "Violet", "Aqua",
                                  "Magenta", "Purple", "Orange", "Orchid"  , "Teal" };

        public List<string> Colors
        {
            get { return _colors; }
            set 
            {
                _colors = value;
                NotifyOfPropertyChange(() => Colors);
            }
        }

        private string _selectedColor;

        public string SelectedColor
        {
            get { return _selectedColor; }
            set 
            {
                _selectedColor = value;
                NotifyOfPropertyChange(() => SelectedColor);
            }
        }


        private string _chartPrice;

        public string ChartPrice
        {
            get { return _chartPrice; }
            set
            {
                _chartPrice = value;
                NotifyOfPropertyChange(() => ChartPrice);
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

        private string _chartSearch;

        public string ChartSearch
        {
            get { return _chartSearch; }
            set
            {
                _chartSearch = value;
                NotifyOfPropertyChange(() => ChartSearch);
            }
        }

        private BindingList<string> _chartRange = new BindingList<string> { "1d", "5d", "1mo", "3mo", "6mo", "1y", "5y" };

        public BindingList<string> ChartRange
        {
            get { return _chartRange; }
            set
            {
                _chartRange = value;
                NotifyOfPropertyChange(() => ChartRange);
            }
        }

        private List<string> _chartInterval;

        public List<string> ChartInterval
        {
            get
            { 
                if(SelectedChartRange != null)
                {
                    if (SelectedChartRange == "1d")
                    {
                        return _chartInterval = new List<string> { "1m", "5m", "15m" };
                    }
                    else if (SelectedChartRange == "5d")
                    {
                        return _chartInterval = new List<string> { "1m", "5m", "15m" };
                    }
                    else if (SelectedChartRange == "1mo")
                    {
                        return _chartInterval = new List<string> { "5m", "15m" , "1d" };
                    }
                    else if (SelectedChartRange == "3mo")
                    {
                        return _chartInterval = new List<string> { "1d", "1wk" };
                    }
                    else if (SelectedChartRange == "6mo")
                    {
                        return _chartInterval = new List<string> { "1d", "1wk" };
                    }
                    else if (SelectedChartRange == "1y")
                    {
                        return _chartInterval = new List<string> { "1d", "1wk", "1mo" };
                    }
                    else if (SelectedChartRange == "5y")
                    {
                        return _chartInterval = new List<string> { "1d", "1wk" , "1mo"};
                    }

                }

                return _chartInterval;
            }
            set
            {
                _chartInterval = value;
                NotifyOfPropertyChange(() => ChartInterval);
            }
        }

        private BindingList<string> _indicators = new BindingList<string> { "EMA", "SMA","MACD" , "Regression" };

        public BindingList<string> Indicators
        {
            get { return _indicators; }
            set 
            {
                _indicators = value;
                NotifyOfPropertyChange(() => Indicators);
            }
        }


        private string _selectedChartRange;

        public string SelectedChartRange
        {
            get { return _selectedChartRange; }
            set
            {
                _selectedChartRange = value;
                NotifyOfPropertyChange(() => SelectedChartRange);
                NotifyOfPropertyChange(() => ChartInterval);
            }
        }

        private string _selectedChartInterval;

        public string SelectedChartInterval
        {
            get { return _selectedChartInterval; }
            set
            {
                _selectedChartInterval = value;
                NotifyOfPropertyChange(() => SelectedChartInterval);

            }
        }

        private BindingList<IndicatorDisplayModel> _addedIndicators = new BindingList<IndicatorDisplayModel>();    

        public BindingList<IndicatorDisplayModel> AddedIndicators
        {
            get { return _addedIndicators; }
            set 
            { 
                _addedIndicators = value;
                NotifyOfPropertyChange(() => AddedIndicators);
            }
        }

        private IndicatorDisplayModel _selectedAddedIndicator;

        public IndicatorDisplayModel SelectedAddedIndicator
        {
            get { return _selectedAddedIndicator; }
            set
            { 
                _selectedAddedIndicator = value;
                NotifyOfPropertyChange(() => SelectedAddedIndicator);
            }
        }

        private string _selectedIndicator;

        public string SelectedIndicator
        {
            get { return _selectedIndicator; }
            set 
            {
                _selectedIndicator = value;
                NotifyOfPropertyChange(() => SelectedIndicator);
                NotifyOfPropertyChange(() => IndicatorInterval);
            }
        }

        private CrossoverTransactionModel _selectedTransaction;

        public CrossoverTransactionModel SelectedTransaction
        {
            get { return _selectedTransaction; }
            set 
            {
                _selectedTransaction = value;
                NotifyOfPropertyChange(() => SelectedTransaction);
            }
        }

        private BindingList<CrossoverTransactionModel> _crossoverTransactions;

        public BindingList<CrossoverTransactionModel> CrossoverTransactions
        {
            get { return _crossoverTransactions; }
            set 
            {
                _crossoverTransactions = value;
                NotifyOfPropertyChange(() => CrossoverTransactions);
            }
        }

        private List<decimal> _prices;

        public List<decimal> Prices
        {
            get { return _prices; }
            set
            { 
                _prices = value;
                NotifyOfPropertyChange(() => Prices);
            }
        }

        private List<ChartPointModel> _buys;

        public List<ChartPointModel> Buys
        {
            get { return _buys; }
            set 
            { 
                _buys = value;
                NotifyOfPropertyChange(() => Buys);
            }
        }

        private List<ChartPointModel> _sells;

        public List<ChartPointModel> Sells
        {
            get { return _sells; }
            set 
            { 
                _sells = value;
                NotifyOfPropertyChange(() => Sells);
            }
        }

        private int _sellShares = 0;

        public int SellShares
        {
            get { return _sellShares; }
            set 
            {
                _sellShares = value;
                NotifyOfPropertyChange(() => SellShares);
            }
        }

        private int _buyShares = 0;

        public int BuyShares
        {
            get { return _buyShares; }
            set 
            {
                _buyShares = value;
                NotifyOfPropertyChange(() => BuyShares);
            }
        }

        private string _profitLoss;

        public string ProfitLoss
        {
            get { return _profitLoss; }
            set 
            {
                _profitLoss = value;
                NotifyOfPropertyChange(() => ProfitLoss);
            }
        }

        private string _newStrategy;

        public string NewStrategy
        {
            get { return _newStrategy; }
            set 
            {
                _newStrategy = value;
                NotifyOfPropertyChange(() => NewStrategy);
            }
        }

        public void Clear ()
        {
            AddedIndicators.Clear();
        }

        public void Delete()
        {
            if(SelectedAddedIndicator == null)
            {
                return;
            }

            AddedIndicators.Remove(SelectedAddedIndicator);
        }

        public void AddIndicator()
        {
            if(SelectedIndicatorInterval == null && SelectedIndicator == null)
            {
                return;
            }

            if(SelectedIndicator == "MACD")
            {
                var indicator = new IndicatorDisplayModel
                {
                    Indicator = "EMA",
                    Interval = "12",
                    Color = "Firebrick"
                };

                AddedIndicators.Add(indicator);

                var indicator2 = new IndicatorDisplayModel
                {
                    Indicator = "EMA",
                    Interval = "26",
                    Color = "Aqua"
                };

                AddedIndicators.Add(indicator2);
            }
            else
            {
                var indicator = new IndicatorDisplayModel
                {
                    Indicator = SelectedIndicator,
                    Interval = SelectedIndicator == "Regression" ? "" : SelectedIndicatorInterval,
                    Color = SelectedColor
                };
                AddedIndicators.Add(indicator);
            }
            
            NotifyOfPropertyChange(() => AddedIndicators);
        }

        
        public async Task RunIndicators()
        {
            await LoadChart(ChartSearch, SelectedChartRange, SelectedChartInterval);

            if(AddedIndicators.Count == 0)
            {
                return;
            }

            if (SelectedIndicator == "SMA")
            {
                
                foreach (var indicator in AddedIndicators)
                {
                    await LoadSMA(indicator.Interval, indicator.Color);
                }

            }
            else if (SelectedIndicator == "EMA" || SelectedIndicator == "MACD")
            {
                foreach(var indicator in AddedIndicators)
                {
                    await LoadEMA(indicator.Interval, indicator.Color);
                }
            }
            else if (SelectedIndicator == "Regression")
            {
                await LoadRegressionData(AddedIndicators[0].Color);
            }


            if (BuyShares > 0 && SellShares > 0)
            {
                await FindCrossovers();
                await AddTransactionsToChart();
                await GetProfitLoss();
            }

        }

        // function to find crossover between the two passed emas
        public async Task FindCrossovers()
        {
            Buys = new List<ChartPointModel>(); 
            Sells = new List<ChartPointModel>();
            CrossoverTransactions = new BindingList<CrossoverTransactionModel>();

            string firstName = SeriesCollection[1].Title;
            firstName.Remove(firstName.Length -1);

            string secondName = SeriesCollection[1].Title;
            secondName.Remove(secondName.Length - 1);

            var ema1 = new ChartCrossoverModel();

            // figure out what ema is on top vs bottom
            if(IndicatorList[0][0] > IndicatorList[1][0])
            {
                ema1.IsTop = true;     
            }
            else 
            {
                ema1.IsTop = false;
            }

            for(int i = 0; i < IndicatorList[0].Count; i++)
            {
                if(ema1.IsTop == true)
                {
                    if(IndicatorList[0][i] <= IndicatorList[1][i])
                    {
                        var crossover = new CrossoverTransactionModel
                        {
                            Index = i > 1 ? i - 1 : i,
                            Date = i > 1 ? Labels[i - 1] : Labels[i],
                            BuyOrSell = "Sell",
                            Price = i > 1 ? Prices[i-1] : Prices[i],
                            Shares = SellShares
                        };
                        var sell = new ChartPointModel
                        {
                            Price = i > 1 ? Prices[i - 1] : Prices[i],
                            Index = i > 1 ? i - 1: i
                        };
                        Sells.Add(sell);
                        CrossoverTransactions.Add(crossover);
                        ema1.IsTop = false;
                    }
                }
                if(ema1.IsTop == false)
                {
                    if (IndicatorList[0][i] >= IndicatorList[1][i])
                    {
                        var crossover = new CrossoverTransactionModel
                        {
                            Index = i > 1 ? i - 1 : i,
                            Date = i > 1 ? Labels[i-1] : Labels[i],
                            BuyOrSell = "Buy",
                            Price = i > 1 ? Prices[i - 1] : Prices[i],
                            Shares = BuyShares
                        };
                        var buy = new ChartPointModel
                        {
                            Price = i > 1 ? Prices[i - 1] : Prices[i],
                            Index = i > 1 ? i - 1 : i
                        };
                        Buys.Add(buy);
                        CrossoverTransactions.Add(crossover);
                        ema1.IsTop = true;
                    }
                }
                
            }
            
        }

        private void AddTransactionsToView()
        {
            CrossoverTransactions.OrderBy(t => t.Index);
        }

        private async Task AddTransactionsToChart()
        {
            var buyValues = new ChartValues<double>();
            var sellValues = new ChartValues<double>();

           
            int buyIdx = 0;
            for (int i = 0; i < ChartLength; i++)
            {
                if (Buys.Exists(item => item.Index == i))
                {
                    buyValues.Add((double)Buys[buyIdx].Price);
                    buyIdx += 1;
                }
                else
                {
                    buyValues.Add(double.NaN);
                }
            }

            int sellIdx = 0;
            for (int i = 0; i < ChartLength; i++)
            {
                if (Sells.Exists(item => item.Index == i))
                {
                    sellValues.Add((double)Sells[sellIdx].Price);
                    sellIdx += 1;
                }
                else
                {
                    sellValues.Add(double.NaN);
                }
            }

            SeriesCollection.Add(
                new LineSeries
                {
                    Values = buyValues,
                    Title = "Buy",
                    Stroke = System.Windows.Media.Brushes.LightSeaGreen,
                    PointGeometrySize = 12,
                    Foreground = Brushes.Black
                });

            SeriesCollection.Add(
                new LineSeries
                {
                    Values = sellValues,
                    Title = "Sell",
                    Stroke = System.Windows.Media.Brushes.Crimson,
                    PointGeometrySize = 12,
                    Foreground = Brushes.Black
                });

        }

        private async Task GetProfitLoss()
        {

            if(CrossoverTransactions.Count <= 1)
            {
                ProfitLoss = "$0";
                return;
            }

            decimal sum = 0;

            if (CrossoverTransactions[0].BuyOrSell == "Buy")
            {

                for(int i = 0; i<CrossoverTransactions.Count; i++)
                {
                    if(CrossoverTransactions[i].BuyOrSell == "Sell")
                    {
                        sum += (CrossoverTransactions[i].Price - CrossoverTransactions[i - 1].Price) * CrossoverTransactions[i].Shares;
                    }
                }
            }
            else
            {
                for(int i = 0; i<CrossoverTransactions.Count; i++)
                {
                    if(CrossoverTransactions[i].BuyOrSell == "Buy")
                    {
                        sum += (CrossoverTransactions[i-1].Price - CrossoverTransactions[i].Price ) * CrossoverTransactions[i].Shares;
                    }
                }
            }

            ProfitLoss = sum.ToString("C");
        }

        public async Task DeleteTransaction()
        {
            if(SelectedTransaction == null)
            {
                return;
            }
            
            //remove from Buys Or Sells List And replot chart
            if(SelectedTransaction.BuyOrSell == "Buy")
            {
                //Buys.RemoveAt(SelectedTransaction.Index);
                foreach(var b in Buys)
                {
                    if(b.Index == SelectedTransaction.Index)
                    {
                        Buys.Remove(b);
                    }
                }

            }
            else
            {
                foreach(var s in Sells)
                {
                    if(s.Index == SelectedTransaction.Index)
                    {
                        Sells.Remove(s);
                        break;
                    }
                }
            }

            //TODO FIGURE OUT NOT UPDATING
            if (SelectedTransaction != null)
            {
                CrossoverTransactions.Remove(SelectedTransaction);
                NotifyOfPropertyChange(() => CrossoverTransactions);
            }

            await UpdateAfterChange();
        }

        private async Task UpdateAfterChange()
        {
           // await LoadChart(ChartSearch, SelectedChartRange, SelectedChartInterval);

            if (SelectedIndicator == "SMA")
            {

                foreach (var indicator in AddedIndicators)
                {
                    await LoadSMA(indicator.Interval, indicator.Color);
                }

            }
            else if (SelectedIndicator == "SMA")
            {

                foreach (var indicator in AddedIndicators)
                {
                    await LoadSMA(indicator.Interval, indicator.Color);
                }

            }
            await AddTransactionsToChart();
            await GetProfitLoss();

            NotifyOfPropertyChange(() => ProfitLoss);
        }

        public async Task CreateNew()
        {
            decimal profitLoss;
            decimal.TryParse(ProfitLoss, NumberStyles.Currency,
            CultureInfo.CurrentCulture.NumberFormat, out profitLoss);
            if (AddedIndicators.Count == 2)
            {
                var strategy = new StrategyModel
                {
                    Name = NewStrategy,
                    MA1 = AddedIndicators[0].Interval,
                    MA2 = AddedIndicators[1].Interval,
                    Indicator = AddedIndicators[0].Indicator,
                    Interval = SelectedChartInterval,
                    Range = SelectedChartRange
                };

                var result = await _strategyEndpoint.PostStrategy(strategy);

                var strategyStock = new StrategyItemModel
                {
                    Id = result,
                    Ticker = ChartSymbol,
                    BuyShares = BuyShares,
                    SellShares = SellShares,
                    ProfitLoss = profitLoss
                };

                var response = await _strategyEndpoint.PostStrategyStock(strategyStock);

                DisplayTransactionResponse(response.Header, response.Message);

            }
            }

        public void DisplayTransactionResponse(string header, string message)
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.WindowStyle = WindowStyle.None;
            //settings.Title = "Transaction Status";


            _transactionInfoVM.UpdateMessage(header, message);
            _window.ShowDialog(_transactionInfoVM, null, settings);
        } 

        public void AddToExisting()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.WindowStyle = WindowStyle.None;
            //settings.Title = "Transaction Status";
            settings.Background = Brushes.Transparent;

            _existingStrategiesVM.Ticker = ChartSymbol;
            _existingStrategiesVM.BuyShares = BuyShares;
            _existingStrategiesVM.SellShares = SellShares;
            _existingStrategiesVM.ProfitLoss = Convert.ToDecimal(ProfitLoss);
            _window.ShowDialog(_existingStrategiesVM, null, settings);
            
        }


        public void OpenStrategies()
        {
            _events.PublishOnUIThread(new OpenStrategiesView());
        }

        public void PaperTradeLive()
        {
            _events.PublishOnUIThread(new OpenPaperTradeLiveView());
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
