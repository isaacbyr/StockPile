using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.PortfolioPro
{
    public class FriendProfileViewModel: Screen
    {
        private readonly IRealizedProfitLossEndpoint _realizedPLEndpoint;
        private readonly ITradeRealizedPLEndpoint _tradeRealizedPLEndpoint;
        private readonly IPortfolioEndpoint _portfolioEndpoint;
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly ITradePortfolioEndpoint _tradePortfolioEndpoint;
        private readonly IFriendsEndpoint _friendsEndpoint;

        public SeriesCollection PortfolioProSeriesCollection { get; set; }
        public List<string> PortfolioProLabels { get; set; }

        public SeriesCollection TraderProSeriesCollection { get; set; }
        public List<string> TraderProLabels { get; set; }

        public string FriendId { get; set; }

        public FriendProfileViewModel(IRealizedProfitLossEndpoint realizedPLEndpoint, ITradeRealizedPLEndpoint tradeRealizedPLEndpoint,
            IPortfolioEndpoint portfolioEndpoint, IStockDataEndpoint stockDataEndpoint, ITradePortfolioEndpoint tradePortfolioEndpoint,
            IFriendsEndpoint friendsEndpoint)
        {
           _realizedPLEndpoint = realizedPLEndpoint;
           _tradeRealizedPLEndpoint = tradeRealizedPLEndpoint;
           _portfolioEndpoint = portfolioEndpoint;
           _stockDataEndpoint = stockDataEndpoint;
           _tradePortfolioEndpoint = tradePortfolioEndpoint;
           _friendsEndpoint = friendsEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadFriendData();
            await LoadPortfolio();
            await LoadTradePortfolio();
            await LoadPortfolioProRealizedPL();
            await LoadTraderProRealizedPL();

        }

        private async Task LoadFriendData()
        {
           var friend = await _friendsEndpoint.LoadFriendById(FriendId);
           UserName = friend.FirstName + " " + friend.LastName + "'s Profile";
        }

        private async Task LoadTraderProRealizedPL()
        {
            var results = await _tradeRealizedPLEndpoint.LoadHistoryByUserId(FriendId);

            if (results.Count > 0)
            {
                var Values = new ChartValues<decimal>();
                PortfolioProLabels = new List<string>();

                foreach (var r in results)
                {
                    Values.Add(r.TotalRealized);
                    PortfolioProLabels.Add(r.Date.ToString("MMM dd"));

                    PortfolioProSeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Values = Values,
                        Title = "Realized P/L",
                        Fill = System.Windows.Media.Brushes.Transparent
                    }
                };

                    NotifyOfPropertyChange(() => PortfolioProSeriesCollection);
                    NotifyOfPropertyChange(() => PortfolioProLabels);
                }
            }
        }

        private async Task LoadPortfolioProRealizedPL()
        {

            var results = await _realizedPLEndpoint.LoadHistoryByUserId(FriendId);

            if (results.Count > 0)
            {
                var Values = new ChartValues<decimal>();
                PortfolioProLabels = new List<string>();

                foreach (var r in results)
                {
                    Values.Add(r.TotalRealized);
                    PortfolioProLabels.Add(r.Date.ToString("MMM dd"));

                    PortfolioProSeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Values = Values,
                        Title = "Realized P/L",
                        Fill = System.Windows.Media.Brushes.Transparent
                    }
                };

                    NotifyOfPropertyChange(() => PortfolioProSeriesCollection);
                    NotifyOfPropertyChange(() => PortfolioProLabels);
                }
            }
        }


        private async  Task LoadTradePortfolio()
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
                        AveragePrice = Math.Round(portfolio[i].AveragePrice, 2)
                    };
                    portfolioStocks.Add(stock);
                }

                TraderProStocks = new BindingList<PortfolioStockDisplayModel>(portfolioStocks);
            }

        }

        private async Task LoadPortfolio()
        {
            var portfolio = await _portfolioEndpoint.LoadPortfolioStocks();

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
                        AveragePrice = Math.Round(portfolio[i].AveragePrice, 2)
                    };
                    portfolioStocks.Add(stock);
                }

                PortfolioStocks = new BindingList<PortfolioStockDisplayModel>(portfolioStocks);
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

        private BindingList<PortfolioStockDisplayModel> _traderProStocks;

        public BindingList<PortfolioStockDisplayModel> TraderProStocks
        {
            get { return _traderProStocks; }
            set 
            {
                _traderProStocks = value;
                NotifyOfPropertyChange(() => TraderProStocks);
            }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set 
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public void Close()
        {
            TryClose();
        }
    }
}
