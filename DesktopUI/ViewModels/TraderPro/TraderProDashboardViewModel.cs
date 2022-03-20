using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.EventModels.TraderPro;
using DesktopUI.Library.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace DesktopUI.ViewModels.TraderPro
{
    public class TraderProDashboardViewModel : Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly IWatchListEndpoint _watchListEndpoint;
        private readonly ITradePortfolioEndpoint _tradePortfolioEndpoint;
        private readonly IUserAccountEndpoint _userAccountEndpoint;
        private readonly IFriendsEndpoint _friendsEndpoint;
        private readonly ITradeTransactionEndpoint _tradeTransactionEndpoint;
        private readonly INewsEndpoint _newsEndpoint;
        private readonly IEventAggregator _events;

        public SeriesCollection LeftSeriesCollection { get; set; }
        public List<string> LeftLabels { get; set; }

        public SeriesCollection RightSeriesCollection { get; set; }
        public List<string> RightLabels { get; set; }

        public SeriesCollection TopHoldingsSeriesCollection { get; set; }
        public List<string> TopHoldingsLabels { get; set; }
        public Func<int, string> Formatter { get; set; }

        public SeriesCollection PieSeriesCollection { get; set; }

        public TraderProDashboardViewModel(IStockDataEndpoint stockDataEndpoint, IWatchListEndpoint watchListEndpoint,
            ITradePortfolioEndpoint tradePortfolioEndpoint, IUserAccountEndpoint userAccountEndpoint,
            IFriendsEndpoint friendsEndpoint, ITradeTransactionEndpoint tradeTransactionEndpoint,
            INewsEndpoint newsEndpoint, IEventAggregator events)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _watchListEndpoint = watchListEndpoint;
            _tradePortfolioEndpoint = tradePortfolioEndpoint;
            _userAccountEndpoint = userAccountEndpoint;
            _friendsEndpoint = friendsEndpoint;
            _tradeTransactionEndpoint = tradeTransactionEndpoint;
            _newsEndpoint = newsEndpoint;
            _events = events;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadLeftChartData("spy");
            await LoadRightChartData("^dji");
            await LoadWatchListData();
            await LoadPortfolioData();
            await LoadDailyGainers();
            await LoadDailyLosers();
            await LoadMarketNews("amzn+aapl+wmt+fb");
            await LoadPortfolioOverview();
            await LoadFriends();
            await LoadDashboard();
            await LoadTopHoldings();
            StartClock();
        }

        private async Task LoadTopHoldings()
        {
            List<SolidColorBrush> colors = new List<SolidColorBrush> { Brushes.AliceBlue, Brushes.CadetBlue, Brushes.LightBlue };

            var results = await _tradePortfolioEndpoint.LoadTopHoldings();

            int index = 0;
            TopHoldingsSeriesCollection = new SeriesCollection();
            TopHoldingsLabels = new List<string>();

            foreach (var r in results)
            {
                var column = new ColumnSeries()
                {
                    Values = new ChartValues<int> { r.Shares },
                    Title = r.Ticker,
                    Fill = colors[index],
                };

                TopHoldingsSeriesCollection.Add(column);
                TopHoldingsLabels.Add(r.Ticker);
                index++;

            }
            Formatter = value => value.ToString("N");

            NotifyOfPropertyChange(() => TopHoldingsLabels);
            NotifyOfPropertyChange(() => TopHoldingsSeriesCollection);

        }

        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("t");
        }


        private async Task LoadDashboard()
        {
            if (Friends == null || Friends.Count == 0)
            {
                return;
            }

            Dashboard = new List<TransactionDisplayModel>();

            foreach (var f in Friends)
            {
                var friendTransactions = await _tradeTransactionEndpoint.LoadTransactionsById(f.Id);

                var convertedTransactions = ConvertToDashboardDisplay(friendTransactions);

                Dashboard.AddRange(convertedTransactions);
            }
            Dashboard = Dashboard.OrderByDescending(x => x.Date).ToList();
        }

        private List<TransactionDisplayModel> ConvertToDashboardDisplay(List<SocialDashboardDataModel> friendTransactions)
        {
            var userTransactions = new List<TransactionDisplayModel>();

            foreach (var t in friendTransactions)
            {
                if (t.Buy == true)
                {
                    var transaction = new TransactionDisplayModel
                    {
                        Date = t.Date,
                        Transaction = $"{t.FullName} bought {t.Shares} shares of {t.Ticker} at {Math.Round(t.Price, 2)}"
                    };

                    userTransactions.Add(transaction);
                }
                else
                {
                    var transaction = new TransactionDisplayModel
                    {
                        Date = t.Date,
                        Transaction = $"{t.FullName} sold {t.Shares} shares of {t.Ticker} at {Math.Round(t.Price, 2)}"
                    };

                    userTransactions.Add(transaction);
                }
            }
            return userTransactions;
        }

        private async Task LoadFriends()
        {
            var friends = await _friendsEndpoint.LoadFriends();

            if (friends.Count > 0)
            {
                Friends = new BindingList<FriendModel>(friends);
            }
        }

        private async Task LoadPortfolioOverview()
        {
            List<SolidColorBrush> colors = new List<SolidColorBrush>
            { Brushes.AliceBlue, Brushes.LightBlue, Brushes.CadetBlue, Brushes.CornflowerBlue, Brushes.DodgerBlue, Brushes.LightSkyBlue,
                Brushes.LightSteelBlue, Brushes.PaleTurquoise, Brushes.PowderBlue, Brushes.SkyBlue, Brushes.DeepSkyBlue, Brushes.PaleTurquoise,
                Brushes.LightCyan, Brushes.AliceBlue, Brushes.LightBlue, Brushes.CadetBlue, Brushes.CornflowerBlue, Brushes.DodgerBlue, Brushes.LightSkyBlue};

            // load account balance and starting amount
            var result = await _userAccountEndpoint.GetTradesPortfolioOverview();
            StartAmount = Math.Round(result.StartAmount, 2);
            AccountBalance = Math.Round(result.AccountBalance, 2);
            RealizedProfitLoss = Math.Round(result.RealizedProfitLoss, 2);

            PieSeriesCollection = new SeriesCollection();

            var cash = new PieSeries()
            {
                Values = new ChartValues<decimal> { result.AccountBalance },
                Title = "Cash",
                Fill = colors[0]
            };

            PieSeriesCollection.Add(cash);

            // Load in pie chart
            if (PortfolioStocks != null)
            {
                NumHoldings = PortfolioStocks.Count();
                UnrealizedProfitLoss = Math.Round((decimal)PortfolioStocks.Sum(s => s.ProfitLoss), 2);

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

        private async Task LoadMarketNews(string query)
        {
            var results = await _newsEndpoint.GetMarketNews(query);

            //substring title
            foreach (var r in results)
            {
                if (r.Title.Length > 75)
                {
                    r.Title = r.Title.Substring(0, 75) + "...";
                }
            }

            Articles = new BindingList<NewsArticleModel>(results);
        }

        private async Task LoadDailyLosers()
        {
            var losers = await _stockDataEndpoint.GetDailyGainersOrLosers("losers");

            if (losers != null)
            {
                DailyLosers = new BindingList<StockDashboardDataModel>(losers);
            }
        }

        private async Task LoadDailyGainers()
        {
            var gainers = await _stockDataEndpoint.GetDailyGainersOrLosers("gainers");

            if (gainers != null)
            {
                DailyGainers = new BindingList<StockDashboardDataModel>(gainers);
            }
        }

        private async Task LoadPortfolioData()
        {
            var portfolio = await _tradePortfolioEndpoint.LoadPortfolioStocks();

            if (portfolio.Count > 0)
            {
                string query = ConvertStocksIntoQuery(portfolio);
                var stockData = await LoadMultipleStockData(query);

                var portfolioStocks = new List<PortfolioStockDisplayModel>();

                for (int i = 0; i < stockData.Count; i++)
                {
                    var stock = new PortfolioStockDisplayModel
                    {
                        Ticker = stockData[i].Ticker,
                        Price = stockData[i].MarketPrice,
                        ProfitLoss = Math.Round((decimal)(stockData[i].MarketPrice - portfolio[i].AveragePrice) * portfolio[i].Shares, 2),
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

        private async Task LoadWatchListData()
        {
            var watchlist = await _watchListEndpoint.LoadWatchList();

            if (watchlist.Count > 0)
            {
                string query = ConvertStocksIntoQuery(watchlist);

                var stockData = await LoadMultipleStockData(query);

                foreach (var stock in stockData)
                {
                    stock.PercentChanged = Math.Round(stock.PercentChanged, 2);
                };

                WatchlistStocks = new BindingList<StockDashboardDataModel>(stockData);
            }
        }

        public string ConvertStocksIntoQuery(dynamic stocks)
        {
            string query = "";
            int index = 1;
            foreach (var stock in stocks)
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

        public async Task<List<StockDashboardDataModel>> LoadMultipleStockData(string query)
        {
            var result = await _stockDataEndpoint.GetMultipleStockDashboardData(query);
            return result;
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

        private async Task LoadLeftChartData(string ticker)
        {
            LeftLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts(ticker);

            LeftChartPrice = marketPrice;
            LeftChartStock = symbol;

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double)result.Open, 2), Math.Round((double)result.High, 2), Math.Round((double)result.Low, 2), Math.Round((double)result.Close, 2));
                Values.Add(point);
                LeftLabels.Add(result.Date);

            }

            LeftSeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Title = symbol,
                    Values = Values
                }
            };


            NotifyOfPropertyChange(() => LeftSeriesCollection);
            NotifyOfPropertyChange(() => LeftLabels);
        }

        private async Task LoadRightChartData(string ticker)
        {
            RightLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts(ticker);

            RightChartPrice = marketPrice;
            RightChartStock = symbol;

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double)result.Open, 2), Math.Round((double)result.High, 2), Math.Round((double)result.Low, 2), Math.Round((double)result.Close, 2));
                Values.Add(point);
                RightLabels.Add(result.Date);
            }

            RightSeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Title = symbol,
                    Values = Values
                }
            };

            NotifyOfPropertyChange(() => RightSeriesCollection);
            NotifyOfPropertyChange(() => RightLabels);
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

        private BindingList<StockDashboardDataModel> _dailyLosers;

        public BindingList<StockDashboardDataModel> DailyLosers
        {
            get { return _dailyLosers; }
            set
            {
                _dailyLosers = value;
                NotifyOfPropertyChange(() => DailyLosers);
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

        private BindingList<FriendModel> friends;

        public BindingList<FriendModel> Friends
        {
            get { return friends; }
            set
            {
                friends = value;
                NotifyOfPropertyChange(() => Friends);
            }
        }

        private List<TransactionDisplayModel> _dashboard;

        public List<TransactionDisplayModel> Dashboard
        {
            get { return _dashboard; }
            set
            {
                _dashboard = value;
                NotifyOfPropertyChange(() => Dashboard);
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

        private string _newsSearchInput;

        public string NewsSearchInput
        {
            get { return _newsSearchInput; }
            set
            {
                _newsSearchInput = value;
                NotifyOfPropertyChange(() => NewsSearchInput);
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

        public async Task RefreshDailyGainers()
        {
            await LoadDailyGainers();
        }

        public async Task RefreshDailyLosers()
        {
            await LoadDailyLosers();
        }

        public void Article_View()
        {
            if (SelectedArticle == null)
            {
                return;
            }
            Process.Start(SelectedArticle.Url);
        }

        public async Task SearchNews()
        {
            await LoadMarketNews(NewsSearchInput);
        }

        public void ViewMorePortfolioSummary()
        {
            _events.PublishOnUIThread(new OpenPortfolioSummaryView());
        }


        public void ViewMoreSocial()
        {
            _events.PublishOnUIThread(new OpenSocialView());
        }

        public void TWSStrategies()
        {
            _events.PublishOnUIThread(new OpenTradeStrategyView(false));
        }

        public void Performance()
        {
            _events.PublishOnUIThread(new OpenTraderPerformanceView());
        }

        public void OpenSocial()
        {
            _events.PublishOnUIThread(new OpenSocialView());
        }

        public void TradeCrossovers()
        {
            _events.PublishOnUIThread(new LaunchTraderProEvent());
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
