using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.EventModels;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DesktopUI.ViewModels
{
    public class PortfolioStockViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly IEventAggregator _events;
        private readonly IUserAccountEndpoint _userAccountEndpoint;
        private readonly IPortfolioEndpoint _portfolioEndpoint;

        public string TickerOnLoad { get; set; } = "AAPL";
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public PortfolioStockViewModel(IStockDataEndpoint stockDataEndpoint, IEventAggregator events,
            IUserAccountEndpoint userAccountEndpoint, IPortfolioEndpoint portfolioEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _events = events;
            _userAccountEndpoint = userAccountEndpoint;
            _portfolioEndpoint = portfolioEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadChart(TickerOnLoad);
            await LoadBuyPanel(TickerOnLoad);
            StartClock();
        }

        private async Task LoadBuyPanel(string ticker)
        {
            var result = await _portfolioEndpoint.GetPortfolioStock(ticker);
            
            if(result != null)
            {
                CurrentPositionAveragePrice = result.AveragePrice.ToString();
                CurrentPositionShares = result.Shares.ToString();
            }
            else
            {
                CurrentPositionShares = "0";
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

            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts(ticker, range, interval);

            ChartPrice = marketPrice;
            ChartSymbol = symbol;

            foreach (var result in results)
            {
                var point = new OhlcPoint(Math.Round((double)result.Open, 2), Math.Round((double)result.High, 2), Math.Round((double)result.Low, 2), Math.Round((double)result.Close, 2));
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

        private string _chartSearch = "Ticker";

        public string ChartSearch
        {
            get { return _chartSearch; }
            set 
            { 
                _chartSearch = value;
                NotifyOfPropertyChange(() => ChartSearch);
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

        private string _currentPositionShares;

        public string CurrentPositionShares
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

        public async Task SearchChart()
        {
            await LoadChart(ChartSearch, SelectedChartRange, SelectedChartInterval);
            await LoadBuyPanel(ChartSearch);
        }

        public void ReturnHome()
        {
            _events.PublishOnUIThread(new ReturnHomeEvent());
        }
        
    }
}
