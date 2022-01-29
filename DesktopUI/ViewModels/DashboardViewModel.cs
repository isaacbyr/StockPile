using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.EventModels;
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
using System.Windows.Media;

namespace DesktopUI.ViewModels
{
    public class DashboardViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly INewsEndpoint _newsEndpoint;
        private readonly IWatchListEndpoint _watchListEndpoint;
        private readonly IPortfolioEndpoint _portfolioEndpoint;
        private readonly IUserAccountEndpoint _userAccountEndpoint;
        private readonly IEventAggregator _events;

        public SeriesCollection SpySeriesCollection { get; set; }
        public List<string> SpyLabels { get; set; }

        public SeriesCollection DowSeriesCollection { get; set; }
        public List<string> DowLabels { get; set; }

        public SeriesCollection TopHoldingsSeriesCollection { get; set; }
        public List<string> TopHoldingsLabels { get; set; }
        public Func<int, string> Formatter { get; set; }

        public SeriesCollection PieSeriesCollection { get; set; }


        public DashboardViewModel(IStockDataEndpoint stockDataEndpoint, INewsEndpoint newsEndpoint, 
            IWatchListEndpoint watchListEndpoint, IPortfolioEndpoint portfolioEndpoint, 
            IUserAccountEndpoint userAccountEndpoint, IEventAggregator events)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _newsEndpoint = newsEndpoint;
            _watchListEndpoint = watchListEndpoint;
            _portfolioEndpoint = portfolioEndpoint;
            _userAccountEndpoint = userAccountEndpoint;
            _events = events;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadLeftChartData("spy");
            await LoadRightChartData("^dji");
            await LoadWatchListData();
            await LoadPortfolioData();
            await LoadDailyGainers();
            await LoadMarketNews("amzn+aapl+wmt+fb");
            await LoadPortfolioOverview();
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

        private async Task LoadPortfolioOverview()
        {
            List<SolidColorBrush> colors = new List<SolidColorBrush> 
            { Brushes.AliceBlue, Brushes.LightBlue, Brushes.CadetBlue, Brushes.CornflowerBlue, Brushes.DodgerBlue, Brushes.LightSkyBlue,
                Brushes.LightSteelBlue, Brushes.PaleTurquoise, Brushes.PowderBlue, Brushes.SkyBlue, Brushes.DeepSkyBlue, Brushes.PaleTurquoise,
                Brushes.LightCyan, Brushes.AliceBlue, Brushes.LightBlue, Brushes.CadetBlue, Brushes.CornflowerBlue, Brushes.DodgerBlue, Brushes.LightSkyBlue};

            // load account balance and starting amount
            var result = await _userAccountEndpoint.GetPortfolioOverview();
            StartAmount = Math.Round(result.StartAmount,2);
            AccountBalance = Math.Round(result.AccountBalance,2);
            RealizedProfitLoss = Math.Round(result.RealizedProfitLoss,2);

            PieSeriesCollection = new SeriesCollection();

            var cash = new PieSeries()
            {
                Values = new ChartValues<decimal> { result.AccountBalance },
                Title = "Cash",
                Fill = colors[0]
            };

            PieSeriesCollection.Add(cash);

            // Load in pie chart
            if(PortfolioStocks != null)
            {
                NumHoldings = PortfolioStocks.Count();
                UnrealizedProfitLoss = Math.Round((decimal)PortfolioStocks.Sum(s => s.ProfitLoss),2);

                int index = 1;
                foreach (var stock in PortfolioStocks)
                {

                    var holding = new PieSeries()
                    {
                        Values = new ChartValues<decimal> { stock.AveragePrice * stock.Shares },
                        Title = stock.Ticker,
                        Fill = colors[index]
                    };

                    index++;
                    PieSeriesCollection.Add(holding);

                }
            }
            
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
            
            if(portfolio.Count > 0)
            {
                string query = ConvertStocksIntoQuery(portfolio);
                var stockData = await LoadMultipleStockData(query);

                var portfolioStocks = new List<PortfolioStockDisplayModel>();

                for(int i = 0; i < stockData.Count; i++)
                {
                    //decimal price;
                    //decimal.TryParse(stockData[i].MarketPrice, out price);

                    var stock = new PortfolioStockDisplayModel
                    {
                        Ticker = stockData[i].Ticker,
                        Price = stockData[i].MarketPrice,
                        ProfitLoss = Math.Round((decimal)(stockData[i].MarketPrice - portfolio[i].AveragePrice) * portfolio[i].Shares,2),
                        Shares = portfolio[i].Shares,
                        AveragePrice = portfolio[i].AveragePrice
                    };
                    portfolioStocks.Add(stock);
                }

                PortfolioStocks = new BindingList<PortfolioStockDisplayModel>(portfolioStocks);

            }
            else
            {
                PortfolioStocks = new BindingList<PortfolioStockDisplayModel>();
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

            if(watchlist.Count > 0)
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

            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts(ticker);

            LeftChartPrice = marketPrice;
            LeftChartStock = symbol;

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double) result.Open,2), Math.Round((double)result.High,2), Math.Round((double)result.Low,2), Math.Round((double) result.Close,2));
                Values.Add(point);
                SpyLabels.Add(result.Date);
                
            }

            SpySeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Title = symbol,
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

            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts(ticker);

            RightChartPrice = marketPrice;
            RightChartStock = symbol;

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double)result.Open, 2), Math.Round((double)result.High, 2), Math.Round((double)result.Low, 2), Math.Round((double)result.Close, 2));
                Values.Add(point);
                DowLabels.Add(result.Date);
            }

            DowSeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Title = symbol,
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

        private PortfolioStockDisplayModel _selectedPortfolioStock;

        public PortfolioStockDisplayModel SelectedPortfolioStock
        {
            get { return _selectedPortfolioStock; }
            set 
            { 
                _selectedPortfolioStock = value;
                NotifyOfPropertyChange(() => SelectedPortfolioStock);
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

        private decimal _realizedProfitLoss = 0;

        public decimal RealizedProfitLoss
        {
            get { return _realizedProfitLoss; }
            set 
            {
                _realizedProfitLoss = value;
                NotifyOfPropertyChange(() => RealizedProfitLoss);
            }
        }

        private decimal _unrealizedProfitLoss = 0;

        public decimal UnrealizedProfitLoss
        {
            get { return _unrealizedProfitLoss; }
            set 
            { 
                _unrealizedProfitLoss = value;
                NotifyOfPropertyChange(() => UnrealizedProfitLoss);
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


        private decimal _startAmount;

        public decimal StartAmount
        {
            get { return _startAmount; }
            set 
            {
                _startAmount = value;
                NotifyOfPropertyChange(() => StartAmount);
            }
        }

        private int _numHoldings = 0;

        public int NumHoldings
        {
            get { return _numHoldings; }
            set 
            {
                _numHoldings = value;
                NotifyOfPropertyChange(() => NumHoldings);
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

        public async Task RefreshWatchlist()
        {
            await LoadWatchListData();
        }

        public async Task RefreshPortfolio()
        {
            await LoadPortfolioData();
        }

        public async Task SearchLeftChart()
        {
            await LoadLeftChartData(SearchInputLeftChart);
        }

        public async Task SearchRightChart()
        {
            await LoadRightChartData(SearchInputRightChart);
        }

        public void Performance()
        {
            _events.PublishOnUIThread(new OpenPortfolioSummaryView());
        }

        public void ViewMorePortfolioSummary()
        {
            _events.PublishOnUIThread(new OpenPortfolioSummaryView());
        }

        public void BuyStocks()
        {
            _events.PublishOnUIThread(new OpenPortfolioStockView("AAPL"));
        }

        public void Social()
        {
            _events.PublishOnUIThread(new OpenSocialView());
        }

        public async Task SearchNews()
        {
            await LoadMarketNews(SearchInput);
        }

        public void ViewMorePortoflioSummary()
        {
            _events.PublishOnUIThread(new OpenPortfolioSummaryView());
        }

        public void LoadPortfolioStockView()
        {
            if (SelectedPortfolioStock == null)
            {
                return;
            }

            _events.PublishOnUIThread(new OpenPortfolioStockView(SelectedPortfolioStock.Ticker));
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
