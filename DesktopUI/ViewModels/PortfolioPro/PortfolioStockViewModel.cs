using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DesktopUI.ViewModels
{
    public class PortfolioStockViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly IEventAggregator _events;
        private readonly IUserAccountEndpoint _userAccountEndpoint;
        private readonly IPortfolioEndpoint _portfolioEndpoint;
        private readonly ITransactionEndoint _transactionEndpoint;
        private readonly IWindowManager _window;
        private readonly TransactionInfoViewModel _transactionInfoVM;
        private readonly IWatchListEndpoint _watchlistEndpoint;
        private readonly IApiHelper _apiHelper;

        public string TickerOnLoad { get; set; } = "AAPL";
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public PortfolioStockViewModel(IStockDataEndpoint stockDataEndpoint, IEventAggregator events,
            IUserAccountEndpoint userAccountEndpoint, IPortfolioEndpoint portfolioEndpoint,
            ITransactionEndoint transactionEndpoint, IWindowManager window, TransactionInfoViewModel transactionInfoVM,
            IWatchListEndpoint watchlistEndpoint, IApiHelper apiHelper)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _events = events;
            _userAccountEndpoint = userAccountEndpoint;
            _portfolioEndpoint = portfolioEndpoint;
            _transactionEndpoint = transactionEndpoint;
            _window = window;
            _transactionInfoVM = transactionInfoVM;
            _watchlistEndpoint = watchlistEndpoint;
            _apiHelper = apiHelper;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadChart(TickerOnLoad);
            await LoadBuyPanel(TickerOnLoad);
            await LoadAccountBalance();
            await LoadCompanyOverview(TickerOnLoad);
            ChartSearch = TickerOnLoad;
            StartClock();
        }

        private async Task LoadCompanyOverview(string ticker)
        {
            var result = await _stockDataEndpoint.GetCompanyOverview(ticker);

            if(result != null)
            {
                Symbol = result.Symbol;
                Sector = result.Sector;
                MarketCapitalization = result.MarketCapitalization;
                EBITDA = result.EBITDA;
                PEGRatio = result.PEGRatio;
                PERatio = result.PERatio;
                DividendYield = result.DividendYield;
                EPS = result.EPS;
                Beta = result.Beta;
                TrailingPE = result.TrailingPE;
                ForwardPE = result.ForwardPE;
                SharesOutstanding = result.SharesOutstanding;
            }
            
        }

        private async Task LoadAccountBalance()
        {
            var result = await _userAccountEndpoint.GetPortfolioOverview();
            AccountBalance = Math.Round(result.AccountBalance,2);
        }

        private async Task LoadBuyPanel(string ticker)
        {
            var result = await _portfolioEndpoint.GetPortfolioStock(ticker);
            
            if(result != null)
            {
                CurrentPositionAveragePrice = result.AveragePrice.ToString();
                CurrentPositionShares = result.Shares;
            }
            else
            {
                CurrentPositionShares = 0;
                CurrentPositionAveragePrice = "0";
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


        private async Task LoadChart(string ticker, string range = "3mo", string interval = "1d")
        {
            Labels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();
            var volume = new ChartValues<long>();

            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts(ticker, range, interval);

            ChartPrice = marketPrice;
            ChartSymbol = symbol;

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double)result.Open, 2), Math.Round((double)result.High, 2), Math.Round((double)result.Low, 2), Math.Round((double)result.Close, 2));
                volume.Add(result.Volume);
                Values.Add(point);
                Labels.Add(result.Date);
            }

            SeriesCollection = new SeriesCollection
            {
                new CandleSeries()
                {
                    Title = symbol,
                    Values = Values
                }
            };


            NotifyOfPropertyChange(() => SeriesCollection);
            NotifyOfPropertyChange(() => Labels);
        }

        private string _symbol;

        public string Symbol
        {
            get { return _symbol; }
            set 
            { 
                _symbol = value;
                NotifyOfPropertyChange(() => Symbol);
            }
        }

        private string _sector;

        public string Sector
        {
            get { return _sector; }
            set 
            {
                _sector = value;
                NotifyOfPropertyChange(() => Sector);
            }
        }

        private string _marketCapitalization;

        public string MarketCapitalization
        {
            get { return _marketCapitalization; }
            set 
            {
                _marketCapitalization = value;
                NotifyOfPropertyChange(() => MarketCapitalization);
            }
        }

        private string _ebitda;

        public string EBITDA
        {
            get { return _ebitda; }
            set 
            {
                _ebitda = value;
                NotifyOfPropertyChange(() => EBITDA);
            }
        }

        private string _peRatio;

        public string PERatio
        {
            get { return _peRatio; }
            set 
            {
                _peRatio = value;
                NotifyOfPropertyChange(() => PERatio);
            }
        }

        private string _pegRatio;

        public string PEGRatio
        {
            get { return _pegRatio; }
            set 
            {
                _pegRatio = value;
                NotifyOfPropertyChange(() => PEGRatio);
            }
        }

        private string _dividendYield;

        public string DividendYield
        {
            get { return _dividendYield; }
            set 
            {
                _dividendYield = value;
                NotifyOfPropertyChange(() => DividendYield);
            }
        }

        private string _eps;

        public string EPS
        {
            get { return _eps; }
            set 
            {
                _eps = value;
                NotifyOfPropertyChange(() => EPS);
            }
        }

        private string _beta;

        public string Beta
        {
            get { return _beta; }
            set 
            { 
                _beta = value;
                NotifyOfPropertyChange(() => Beta);
            }
        }

        private string _trailingPE;

        public string TrailingPE
        {
            get { return _trailingPE; }
            set 
            {
                _trailingPE = value;
                NotifyOfPropertyChange(() => TrailingPE);
            }
        }

        private string _forwardPE;

        public string ForwardPE
        {
            get { return _forwardPE; }
            set 
            {
                _forwardPE = value;
                NotifyOfPropertyChange(() => ForwardPE);
            }
        }

        private string _sharesOutstanding;

        public string SharesOutstanding
        {
            get { return _sharesOutstanding; }
            set 
            {
                _sharesOutstanding = value;
                NotifyOfPropertyChange(() => SharesOutstanding);
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
                NotifyOfPropertyChange(() => CanSearchChart);
            }
        }

        private List<string> _chartRange = new List<string> { "1d", "5d", "1mo", "3mo", "6mo", "1y", "5y", "10y", "ytd", "max" };

        public List<string> ChartRange
        {
            get { return _chartRange; }
            set 
            { 
                _chartRange = value;
                NotifyOfPropertyChange(() => ChartRange);
            }
        }

        private List<string> _chartInterval = new List<string> { "1m", "5m", "15m", "1d", "1wk", "1mo" };

        public List<string> ChartInterval
        {
            get { return _chartInterval; }
            set 
            { 
                _chartInterval = value;
                NotifyOfPropertyChange(() => ChartInterval);
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
                NotifyOfPropertyChange(() => CanSearchChart);
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
                NotifyOfPropertyChange(() => CanSearchChart);

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

        private string _curentPositionAveragePrice;

        public string CurrentPositionAveragePrice
        {
            get { return _curentPositionAveragePrice; }
            set 
            {
                _curentPositionAveragePrice = value;
                NotifyOfPropertyChange(() => CurrentPositionAveragePrice);
            }
        }

        private int _newPositionShares;

        public int NewPositionShares
        {
            get { return _newPositionShares; }
            set
            { 
                _newPositionShares = value;
                NotifyOfPropertyChange(() => NewPositionShares);
                NotifyOfPropertyChange(() => CashAmount);
                NotifyOfPropertyChange(() => CanBuy);
                NotifyOfPropertyChange(() => CanSell);
            }
        }

        private decimal _cashAmount;

        public decimal CashAmount
        {
            get 
            {
                decimal price;
                decimal.TryParse(ChartPrice, out price);
                return Math.Round(NewPositionShares * price,2);
            }
            set 
            { 
                _cashAmount = value;
                NotifyOfPropertyChange(() => CanBuy);
            }
        }

        public decimal ProfitLoss
        {
            get
            {
                decimal avgprice;
                decimal.TryParse(CurrentPositionAveragePrice, out avgprice);

                decimal currprice;
                decimal.TryParse(ChartPrice, out currprice);

                return Math.Round((avgprice - currprice) * CurrentPositionShares,2);
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

        public bool CanBuy
        {
            get
            {
                if(CashAmount < AccountBalance) return true;
                else return false;
            }
        }

        public bool CanSell
        {
            get
            {
                if (NewPositionShares <= CurrentPositionShares && NewPositionShares > 0) return true;
                else return false;
            }
        }

        public bool CanSearchChart
        {
            get
            {
                if(SelectedChartRange != null && SelectedChartInterval != null && ChartSearch != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task Buy()
        {
            //change account balance
            await _userAccountEndpoint.UpdatePortfolioAccountBalance(CashAmount);

            //add to transaction table
            decimal price;
            decimal.TryParse(ChartPrice, out price);
            var transaction = new TransactionModel
            {
                Ticker = ChartSymbol,
                Buy = true,
                Price = price,
                Sell = false,
                Shares = NewPositionShares,
                Date = DateTime.Now

            };
            await _transactionEndpoint.PostTransaction(transaction);

            // update portfolio table
            var stock = new PortfolioModel
            {
                Ticker = ChartSymbol,
                Price = price,
                Shares = NewPositionShares
            };

            if(CurrentPositionShares == 0)
            {
                await _portfolioEndpoint.PostStock(stock);
            }
            else
            {
                await _portfolioEndpoint.UpdatePortfolioBuy(stock);
            }

            //Display Popup and Reset Buy Panel
            DisplayTransactionCompletion("Transaction Complete", $"Bought {NewPositionShares} shares of {ChartSymbol} for ${ChartPrice}");
            await ResetBuyPanel();
            await LoadAccountBalance();
        }

        public async Task Sell()
        {
            //add to transaction table
            decimal price;
            decimal.TryParse(ChartPrice, out price);
            var transaction = new TransactionModel
            {
                Ticker = ChartSymbol,
                Buy = false,
                Price = price,
                Sell = true,
                Date = DateTime.Now,
                Shares = NewPositionShares,
            };

            await _transactionEndpoint.PostTransaction(transaction);

            //Update Portfolio
            var stock = new PortfolioModel
            {
                Ticker = ChartSymbol,
                Price = price,
                Shares = NewPositionShares
            };

            // if sold shares equals shares in portfolio delete entry
            decimal realizedProfitLoss;

            if (CurrentPositionShares == NewPositionShares)
            {
               realizedProfitLoss = await _portfolioEndpoint.UpdateAndDeletePortfolio(stock);
            }
            else
            {
                // else update exisitng position
                realizedProfitLoss = await _portfolioEndpoint.UpdatePortfolioSell(stock);
            }

            // update user account table, account balance and realizedgains
            var result = await _userAccountEndpoint.UpdateAfterSale(realizedProfitLoss, CashAmount);
            
            AccountBalance = Math.Round(result,2);
            DisplayTransactionCompletion("Transaction Complete",$"Sold {NewPositionShares} shares of {ChartSymbol} for {ChartPrice}");
            await ResetBuyPanel();

        }

        public void DisplayTransactionCompletion(string header, string message)
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.WindowStyle = WindowStyle.None;
            //settings.Title = "Transaction Status";
            

            _transactionInfoVM.UpdateMessage(header, message);
            _window.ShowDialog(_transactionInfoVM, null, settings);
        }

        private async Task ResetBuyPanel()
        {
            NewPositionShares = 0;
            CashAmount = 0;
            await LoadBuyPanel(ChartSymbol);
        }


        public async Task SearchChart()
        {
            await LoadChart(ChartSearch, SelectedChartRange, SelectedChartInterval);
            await LoadBuyPanel(ChartSearch);
            await LoadCompanyOverview(ChartSearch);
        }

        public async Task AddToWatchlist()
        {
            var result = await _watchlistEndpoint.PostWatchlistStock(ChartSymbol);
            DisplayTransactionCompletion(result.Header, result.Message);
        }

        public void ReturnHome()
        {
            _events.PublishOnUIThread(new ReturnHomeEvent());
        }



        public void Performance()
        {
            _events.PublishOnUIThread(new OpenPortfolioSummaryView());
        }


        public void Home()
        {
            _events.PublishOnUIThread(new ReturnHomeEvent());
        }


        public void OpenSocial()
        {
            _events.PublishOnUIThread(new OpenSocialView());
        }


        public void Logout()
        {
            _apiHelper.Logout();
            _events.PublishOnUIThread(new LogOffEvent());
        }

        public void Exit()
        {
            _events.PublishOnUIThread(new ExitAppEvent());
        }

    }
}
