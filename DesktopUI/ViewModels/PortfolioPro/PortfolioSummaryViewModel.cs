using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace DesktopUI.ViewModels
{
    public class PortfolioSummaryViewModel: Screen
    {
        private readonly IRealizedProfitLossEndpoint _realizedPLEndpoint;
        private readonly IPortfolioEndpoint _portfolioEndpoint;
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly IUserAccountEndpoint _userAccountEndpoint;
        private readonly ITransactionEndoint _transactionEndpoint;
        private readonly IEventAggregator _events;
        private readonly IApiHelper _apiHelper;

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public SeriesCollection PieSeriesCollection { get; set; }

        public SeriesCollection TransactionSeriesCollection { get; set; }
        public List<string> TransactionLabels { get; set; }


        public PortfolioSummaryViewModel(IRealizedProfitLossEndpoint realizedPLEndpoint, IPortfolioEndpoint portfolioEndpoint,
            IStockDataEndpoint stockDataEndpoint, IUserAccountEndpoint userAccountEndpoint, ITransactionEndoint transactionEndpoint,
            IEventAggregator events, IApiHelper apiHelper)
        {
            _realizedPLEndpoint = realizedPLEndpoint;
            _portfolioEndpoint = portfolioEndpoint;
            _stockDataEndpoint = stockDataEndpoint;
            _userAccountEndpoint = userAccountEndpoint;
            _transactionEndpoint = transactionEndpoint;
            _events = events;
            _apiHelper = apiHelper;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadPortfolioStocks();
            await LoadPortfolioOverview();
            await LoadAssetAllocation();
            await LoadRealizedProfitLoss();
            await LoadTransactions();
            StartClock();
        }

        private async Task LoadTransactions()
        {
            TransactionSeriesCollection = new SeriesCollection();
            TransactionLabels = new List<string>();

            var results = await _transactionEndpoint.LoadTransactions();
            Transactions = new BindingList<TransactionDisplayModel>();

            if(results != null)
            {
                foreach(var r in results)
                {
                    string transaction = "";

                    if(r.Buy == true)
                    {
                        transaction = $"Bought {r.Shares} shares of {r.Ticker} at {r.Price}";
                    }
                    else
                    {
                        transaction = $"Sold {r.Shares} shares of {r.Ticker} at {r.Price}";
                    }

                    var transactionDisplay = new TransactionDisplayModel
                    {
                        //TODO: Fix Date
                        Date = r.Date,
                        Transaction = transaction
                    };

                    Transactions.Add(transactionDisplay);
                }
            }

            //load chart
            var transactionChartData = await _transactionEndpoint.LoadChartData();

            if(transactionChartData.Count > 0)
            {
                
                TransactionSeriesCollection.Add(
                    new ColumnSeries
                    {
                        Title = "Count",
                        Values = new ChartValues<int>(transactionChartData.Select(t => t.Count)),
                        Fill = System.Windows.Media.Brushes.CadetBlue
                    });

                foreach (var date in transactionChartData.Select(t => t.Date))
                {
                    TransactionLabels.Add(date.ToString("MMM dd"));
                }
            }

            NotifyOfPropertyChange(() => Transactions);
            NotifyOfPropertyChange(() => TransactionLabels);
            NotifyOfPropertyChange(() => TransactionSeriesCollection);
        }

        private async Task LoadPortfolioOverview()
        {
            var result = await _userAccountEndpoint.GetPortfolioOverview();
            StartAmount = Math.Round(result.StartAmount,2);
            AccountBalance = Math.Round(result.AccountBalance,2);
            RealizedProfitLoss = Math.Round(result.RealizedProfitLoss,2);
        }

        private async Task LoadPortfolioStocks()
        {
            var portfolio = await _portfolioEndpoint.LoadPortfolioStocks();

            if (portfolio.Count > 0)
            {
                string query = ConvertStocksIntoQuery(portfolio);
                var stockData = await LoadMultipleStockData(query);

                var portfolioStocks = new List<PortfolioStockDisplayModel>();

                for (int i = 0; i < stockData.Count; i++)
                {
                    //decimal price;
                    //decimal.TryParse(stockData[i].MarketPrice, out price);

                    var stock = new PortfolioStockDisplayModel
                    {
                        Ticker = stockData[i].Ticker,
                        Price = stockData[i].MarketPrice,
                        ProfitLoss = Math.Round((decimal)(stockData[i].MarketPrice - portfolio[i].AveragePrice) * portfolio[i].Shares, 2),
                        Shares = portfolio[i].Shares,
                        AveragePrice = Math.Round(portfolio[i].AveragePrice,2)
                    };
                    portfolioStocks.Add(stock);
                }

                PortfolioStocks = new BindingList<PortfolioStockDisplayModel>(portfolioStocks);
                NumHoldings = PortfolioStocks.Count();
                UnrealizedProfitLoss = PortfolioStocks.Sum(s => s.ProfitLoss);

                //find top performer
                TopPerformer =  PortfolioStocks.OrderByDescending(s => s.ProfitLoss).FirstOrDefault().Ticker;
                WorstPerformer = PortfolioStocks.OrderBy(s => s.ProfitLoss).FirstOrDefault().Ticker;
            }

        }

        public async Task<List<StockDashboardDataModel>> LoadMultipleStockData(string query)
        {
            var result = await _stockDataEndpoint.GetMultipleStockDashboardData(query);
            return result;
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

        private async Task LoadAssetAllocation()
        {
            List<SolidColorBrush> colors = new List<SolidColorBrush>
            { Brushes.AliceBlue, Brushes.LightBlue, Brushes.CadetBlue, Brushes.CornflowerBlue, Brushes.DodgerBlue, Brushes.LightSkyBlue,
                Brushes.LightSteelBlue, Brushes.PaleTurquoise, Brushes.PowderBlue, Brushes.SkyBlue, Brushes.DeepSkyBlue, Brushes.PaleTurquoise,
                Brushes.LightCyan, Brushes.AliceBlue, Brushes.LightBlue, Brushes.CadetBlue, Brushes.CornflowerBlue, Brushes.DodgerBlue, Brushes.LightSkyBlue};

            
            PieSeriesCollection = new SeriesCollection();

            var cash = new PieSeries()
            {
                Values = new ChartValues<decimal> { AccountBalance },
                Title = "Cash",
                Fill = colors[0]
            };

            PieSeriesCollection.Add(cash);

            if (PortfolioStocks != null)
            {
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

        private async Task LoadRealizedProfitLoss()
        {
            int wins = 0;
            int losses = 0;

            var results = await _realizedPLEndpoint.LoadHistory();

            if (results.Count >0)
            {
                var Values = new ChartValues<decimal>();
                Labels = new List<string>();

                foreach(var r in results)
                {
                    // add to cart
                    Values.Add(r.TotalRealized);
                    Labels.Add(r.Date.ToString("MMM dd"));

                    //find wins vs losses
                    if (r.ProfitLoss > 0)
                    {
                        wins++;
                    }
                    else
                    {
                        losses++;
                    }
                }

                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Values = Values,
                        Title = "Realized P/L",
                        Fill = System.Windows.Media.Brushes.Transparent
                    }
                };

                Wins = wins;
                Losses = losses;

                RealizedProfitLoss = results.Last().TotalRealized;

                NotifyOfPropertyChange(() => WinningPercentage);
                NotifyOfPropertyChange(() => SeriesCollection);
                NotifyOfPropertyChange(() => Labels);
            }
        }


        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += tickevent;
            timer.Start();
        }

        private void tickevent(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("t");
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

        private BindingList<TransactionDisplayModel> _transactions;

        public BindingList<TransactionDisplayModel> Transactions
        {
            get { return _transactions; }
            set 
            { 
                _transactions = value;
                NotifyOfPropertyChange(() => Transactions);
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

        private string _topPerformer;

        public string TopPerformer
        {
            get { return _topPerformer; }
            set 
            {
                _topPerformer = value;
                NotifyOfPropertyChange(() => TopPerformer);
            }
        }

        private string _worstPerformer;

        public string WorstPerformer
        {
            get { return _worstPerformer; }
            set 
            {
                _worstPerformer = value;
                NotifyOfPropertyChange(() => WorstPerformer);
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

        private int _wins;

        public int Wins
        {
            get { return _wins; }
            set 
            { 
                _wins = value;
                NotifyOfPropertyChange(() => Wins);
            }
        }

        private int _losses;

        public int Losses
        {
            get { return _losses; }
            set 
            {
                _losses = value;
                NotifyOfPropertyChange(() => Losses);
            }
        }

        public decimal WinningPercentage
        {
            get
            {
                if(Losses != 0)
                {
                    return Math.Round(((decimal)Wins / (Losses+Wins)) * 100, 2);
                }
                else
                {
                    return 0;
                }
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

        public async Task RefreshPortfolio()
        {
            await LoadPortfolioStocks();
            await LoadPortfolioOverview();
        }

        public void LoadPortfolioStockView()
        {
            if(SelectedPortfolioStock.Ticker != null)
            {
                _events.PublishOnUIThread(new OpenPortfolioStockView(SelectedPortfolioStock.Ticker));
            }
            else
            {
                return;
            }
        }

         public void MainMenu()
        {
            _events.PublishOnUIThread(new OpenMainMenuEvent());
        }

        public void Dashboard()
        {
            _events.PublishOnUIThread(new LaunchPortoflioProEvent());
        }

        public void BuySellStocks()
        {
            _events.PublishOnUIThread(new OpenPortfolioStockView("AAPL"));
        }


        public void OpenSocial()
        {
            _events.PublishOnUIThread(new OpenSocialView());
        }

    }
}
