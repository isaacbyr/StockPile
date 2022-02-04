using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DesktopUI.ViewModels.TraderPro
{
    public class TraderMainViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        public int ChartLength { get; set; }

        public List<List<decimal>> IndicatorList { get; set; } = new List<List<decimal>>();

        public TraderMainViewModel(IStockDataEndpoint stockDataEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadChart("AAPL");
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
                Prices.Add( Math.Round((result.Open + result.High + result.Low + result.Close) / 4 ,2));
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

                var (results, symbol, marketPrice) = await _stockDataEndpoint.GetSMAChartData(ChartSearch, range, SelectedChartInterval, lastResults);
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
            return ("", 0);
        }


        private List<string> _indicatorInterval = new List<string> { "4", "9", "13", "21", "50" };

        public List<string> IndicatorInterval
        {
            get { return _indicatorInterval; }
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

        private List<string> _colors = new List<string> { "CadetBlue", "Turquoise" , "Gold", "Fuchsia", "Violet", "Aqua",
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

        private List<string> _chartRange = new List<string> { "1d", "5d", "1mo", "3mo", "6mo", "1y", "5y" };

        public List<string> ChartRange
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

        private List<string> _indicators = new List<string> { "EMA", "SMA", "Regression", "SVM" };

        public List<string> Indicators
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
            }
        }

        private List<CrossoverTransactionModel> _crossoverTransactions = new List<CrossoverTransactionModel>();

        public List<CrossoverTransactionModel> CrossoverTransactions
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
            if(SelectedIndicatorInterval == null || SelectedIndicator == null)
            {
                return;
            }

            var indicator = new IndicatorDisplayModel
            {
                Indicator = SelectedIndicator,
                Interval = SelectedIndicatorInterval,
                Color = SelectedColor
            };

            AddedIndicators.Add(indicator);

            NotifyOfPropertyChange(() => AddedIndicators);
        }

        
        public async Task RunIndicators()
        {
            await LoadChart(ChartSearch, SelectedChartRange, SelectedChartInterval);

            if (SelectedIndicator == "SMA")
            {
                
                foreach (var indicator in AddedIndicators)
                {
                    await LoadSMA(indicator.Interval, indicator.Color);
                }

            }
            else if (SelectedIndicator == "EMA")
            {

            }

            FindCrossovers();
        }

        public void FindCrossovers()
        {
            Buys = new List<ChartPointModel>(); 
            Sells = new List<ChartPointModel>();

            string firstName = SeriesCollection[1].Title;
            firstName.Remove(firstName.Length -1);

            string secondName = SeriesCollection[1].Title;
            secondName.Remove(secondName.Length - 1);

            var ema1 = new ChartCrossoverModel();

            // figure out what ema is on top vs bottom
            if(IndicatorList[0][0] > IndicatorList[1][0])
            {
                ema1.Name = firstName;
                ema1.IsTop = true;     
            }
            else 
            {
                ema1.Name = firstName;
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
                            BuyOrSell = "Sell",
                            Price = Prices[i],
                            Shares = 100
                        };
                        var sell = new ChartPointModel
                        {
                            Price = Prices[i],
                            Index = i
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
                            BuyOrSell = "Buy",
                            Price = Prices[i],
                            Shares = 100
                        };
                        var buy = new ChartPointModel
                        {
                            Price = Prices[i],
                            Index = i
                        };
                        Buys.Add(buy);
                        CrossoverTransactions.Add(crossover);
                        ema1.IsTop = true;
                    }
                }
                
            }
            AddTransactionsToChart();
        }

        private void AddTransactionsToChart()
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

            int test = 0;

            SeriesCollection.Add(
                new LineSeries
                {
                    Values = buyValues,
                    Title = "Buys",
                    //Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = System.Windows.Media.Brushes.Chartreuse,
                    //StrokeThickness = 4,
                    PointGeometrySize = 15,
                    PointGeometry = DefaultGeometries.Diamond
                });

            SeriesCollection.Add(
                new LineSeries
                {
                    Values = sellValues,
                    Title = "Sells",
                    //Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = System.Windows.Media.Brushes.Crimson,
                    //StrokeThickness = 4,
                    //PointGeometrySize = 30,
                    PointGeometrySize = 15,
                    PointGeometry = DefaultGeometries.Diamond,

                });

        }
    }
}
