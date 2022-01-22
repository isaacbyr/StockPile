using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DesktopUI.ViewModels
{
    public class DashboardViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly INewsEndpoint _newsEndpoint;
        private readonly IWatchListEndpoint _watchListEndpoint;
        private readonly IPortfolioEndpoint _portfolioEndpoint;

        public SeriesCollection SpySeriesCollection { get; set; }
        public List<string> SpyLabels { get; set; }

        public SeriesCollection DowSeriesCollection { get; set; }
        public List<string> DowLabels { get; set; }

        public SeriesCollection TopHoldingsSeriesCollection { get; set; }
        public List<string> TopHoldingsLabels { get; set; }
        public Func<int, string> Formatter { get; set; }

        public SeriesCollection PieSeriesCollection { get; set; }


        public DashboardViewModel(IStockDataEndpoint stockDataEndpoint, INewsEndpoint newsEndpoint, 
            IWatchListEndpoint watchListEndpoint, IPortfolioEndpoint portfolioEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _newsEndpoint = newsEndpoint;
            _watchListEndpoint = watchListEndpoint;
            _portfolioEndpoint = portfolioEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            //await LoadLeftChartData("spy");
            //await LoadRightChartData("^dji");
            //await LoadWatchListData();
            //await LoadPortfolioData();
            //await LoadDailyGainers();
            await LoadMarketNews("amzn+aapl+wmt+fb");
            LoadAccountPie();
            LoadTopHoldings();
            StartClock();
        }

        private void LoadTopHoldings()
        {
            TopHoldingsSeriesCollection = new SeriesCollection
            {
                new ColumnSeries()
                {
                    Values = new ChartValues<double> {10000},
                    Title = "AAPL", 
                    Fill = System.Windows.Media.Brushes.AliceBlue
                },
                new ColumnSeries()
                {
                    Values = new ChartValues<double> {11800},
                    Title = "NFLX",
                    Fill = System.Windows.Media.Brushes.CadetBlue
                },
                new ColumnSeries()
                {
                    Values = new ChartValues<double> {7500},
                    Title = "LULU",
                    Fill = System.Windows.Media.Brushes.LightBlue
                },
            };

            TopHoldingsLabels = new List<string>{ "AAPL", "NFLX", "LULU"};
            Formatter = value => value.ToString("N");

            NotifyOfPropertyChange(() => TopHoldingsLabels);
            NotifyOfPropertyChange(() => TopHoldingsSeriesCollection);
            
        }

        private void LoadAccountPie()
        {
            PieSeriesCollection = new SeriesCollection
            {
                new PieSeries()
                {
                    Values = new ChartValues<decimal> {1},
                    Title = "Cash",
                    Fill = System.Windows.Media.Brushes.AliceBlue
                },
                new PieSeries()
                {
                    Values = new ChartValues<decimal> {2},
                    Title = "AAPL",
                    Fill = System.Windows.Media.Brushes.CadetBlue
                },
                new PieSeries()
                {
                    Values = new ChartValues<decimal> {3},
                    Title = "NFLX",
                    Fill = System.Windows.Media.Brushes.LightBlue
                },
            };

            NotifyOfPropertyChange(() => PieSeriesCollection);
        }

        private async Task LoadDailyGainers()
        {
            var gainers = await _stockDataEndpoint.GetDailyGainers();

            if(gainers != null)
            {
                DailyGainers = new BindingList<StockDashboardDataModel>(gainers);
            }
        }

        private async Task LoadPortfolioData()
        {
            var portfolio = await _portfolioEndpoint.LoadPortfolioStocks();
            
            if(portfolio != null)
            {
                string query = ConvertStocksIntoQuery(portfolio);
                var stockData = await LoadMultipleStockData(query);

                var portfolioStocks = new List<PortfolioStockDisplayModel>();

                for(int i = 0; i < stockData.Count; i++)
                {
                    decimal price;
                    decimal.TryParse(stockData[i].MarketPrice, out price);

                    var stock = new PortfolioStockDisplayModel
                    {
                        Ticker = stockData[i].Ticker,
                        Price = stockData[i].MarketPrice,
                        ProfitLoss = (double)(price - portfolio[i].AveragePrice) * portfolio[i].Shares
                    };
                    portfolioStocks.Add(stock);
                }

                PortfolioStocks = new BindingList<PortfolioStockDisplayModel>(portfolioStocks);

            }
        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void tickevent(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("t");
        }

        private async Task LoadWatchListData()
        {
            
            var watchlist = await _watchListEndpoint.LoadWatchList();

            if(watchlist != null)
            {
                var watchlistData = new List<WatchlistDisplayModel>();

                string query = ConvertStocksIntoQuery(watchlist);

                var stockData = await LoadMultipleStockData(query);

                foreach(var stock in stockData)
                {
                    decimal temp;
                    decimal.TryParse(stock.PercentChanged, out temp);
                    temp = Math.Round(temp, 3);
                    stock.PercentChanged = temp.ToString("");
                }

                WatchlistStocks = new BindingList<StockDashboardDataModel>(stockData);

                
            }

        }

        public string ConvertStocksIntoQuery(dynamic stocks)
        {
            string query = "";
            int index = 1;
            foreach(var stock in stocks)
            {
                if (index < stocks.Count)
                {
                    query = query + stock.Ticker + "%2C";
                }
                else
                {
                    query = query + stock.Ticker;
                }

                index++;
            }

            return query;
        }

        private async Task LoadMarketNews(string query)
        {
            var results = await _newsEndpoint.GetMarketNews(query);

            // substring title
            foreach(var r in results)
            {
                if(r.Title.Length > 75)
                {
                    r.Title = r.Title.Substring(0, 75) + "...";
                }
            }

            Articles = new BindingList<NewsArticleModel>(results);

        }

        private async Task LoadLeftChartData(string ticker)
        {
            SpyLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var stockData = await LoadStockData(ticker);
            LeftChartPrice = stockData.MarketPrice;
            LeftChartStock = stockData.Ticker;

            var results = await _stockDataEndpoint.GetDashboardCharts(ticker);

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double) result.Open,2), Math.Round((double)result.High,2), Math.Round((double)result.Low,2), Math.Round((double) result.Close,2));
                Values.Add(point);
                
            }

            SpySeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Values = Values
                }
            };

            NotifyOfPropertyChange(() => SpySeriesCollection);
            NotifyOfPropertyChange(() => SpyLabels);
        }

        private async Task LoadRightChartData(string ticker)
        {
            DowLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var stockData = await LoadStockData(ticker);
            RightChartPrice = stockData.MarketPrice;
            RightChartStock = stockData.Ticker;

            var results = await _stockDataEndpoint.GetDashboardCharts(ticker);

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double)result.Open, 2), Math.Round((double)result.High, 2), Math.Round((double)result.Low, 2), Math.Round((double)result.Close, 2));
                Values.Add(point);
                //DowLabels.Add(result.Date);
            }

            DowSeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Values = Values
                }
            };

            NotifyOfPropertyChange(() => DowSeriesCollection);
            NotifyOfPropertyChange(() => DowLabels);
        }

        public async Task<List<StockDashboardDataModel>> LoadMultipleStockData(string query)
        {
            var result = await _stockDataEndpoint.GetMultipleStockDashboardData(query);
            return result;
        }

        public async Task<StockDashboardDataModel> LoadStockData(string ticker)
        {
            var result = await _stockDataEndpoint.GetStockDashboardData(ticker);
            return result;
        }

        private BindingList<StockDashboardDataModel> _dailyGainers;

        public BindingList<StockDashboardDataModel> DailyGainers
        {
            get { return _dailyGainers; }
            set 
            {
                _dailyGainers = value;
                NotifyOfPropertyChange(() => DailyGainers);
            }
        }


        private BindingList<PortfolioStockDisplayModel> _portfolioStocks;

        public BindingList<PortfolioStockDisplayModel> PortfolioStocks
        {
            get { return _portfolioStocks; }
            set 
            {
                _portfolioStocks = value;
                NotifyOfPropertyChange(() => PortfolioStocks);
            }
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


        private string _leftChartStock;

        public string LeftChartStock
        {
            get { return _leftChartStock; }
            set 
            { 
                _leftChartStock = value;
                NotifyOfPropertyChange(() => LeftChartStock);
            }
        }

        private string _rightChartStock;

        public string RightChartStock
        {
            get { return _rightChartStock; }
            set 
            { 
                _rightChartStock = value;
                NotifyOfPropertyChange(() => RightChartStock);
            }
        }

        private string _rightChartPrice;

        public string RightChartPrice
        {
            get { return _rightChartPrice; }
            set 
            { 
                _rightChartPrice = value;
                NotifyOfPropertyChange(() => RightChartPrice);
            }
        }

        private string _searchInputLeftChart;

        public string SearchInputLeftChart
        {
            get { return _searchInputLeftChart; }
            set 
            { 
                _searchInputLeftChart = value;
                NotifyOfPropertyChange(() => SearchInputLeftChart);
            }
        }

        private string _searchInputRightChart;

        public string SearchInputRightChart
        {
            get { return _searchInputRightChart; }
            set 
            {
                _searchInputRightChart = value;
                NotifyOfPropertyChange(() => SearchInputRightChart);
            }
        }


        private BindingList<NewsArticleModel> _articles;

        public BindingList<NewsArticleModel> Articles
        {
            get { return _articles; }
            
            set 
            { 
                _articles = value;
                NotifyOfPropertyChange(() => Articles);
            }
        }

        private BindingList<StockDashboardDataModel> _watchlistStocks;

        public BindingList<StockDashboardDataModel> WatchlistStocks
        {
            get { return _watchlistStocks; }
            set 
            { 
                _watchlistStocks = value;
                NotifyOfPropertyChange(() => WatchlistStocks);
            }
        }


        private NewsArticleModel _selectedArticle;

        public NewsArticleModel SelectedArticle
        {
            get { return _selectedArticle; }
            set 
            { 
                _selectedArticle = value;
                NotifyOfPropertyChange(() => SelectedArticle);
            }
        }

        private string _searchInput = "AAPL";

        public string SearchInput
        {
            get { return _searchInput; }
            set 
            {
                _searchInput = value;
                NotifyOfPropertyChange(() => SearchInput);
            }
        }

        private string _leftChartPrice;

        public string LeftChartPrice
        {
            get { return _leftChartPrice; }
            set 
            {
                _leftChartPrice = value;
                NotifyOfPropertyChange(() => LeftChartPrice);
            }
        }

        public async Task SearchLeftChart()
        {
            await LoadLeftChartData(SearchInputLeftChart);
        }

        public async Task SearchRightChart()
        {
            await LoadRightChartData(SearchInputRightChart);
        }

        public async Task SearchNews()
        {
            await LoadMarketNews(SearchInput);
        }

        public void Article_View()
        {
            if(SelectedArticle == null)
            {
                return;
            }
            Process.Start(SelectedArticle.Url);
        }
    }
}
