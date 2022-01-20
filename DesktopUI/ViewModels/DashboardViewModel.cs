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

namespace DesktopUI.ViewModels
{
    public class DashboardViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;
        private readonly INewsEndpoint _newsEndpoint;
        private readonly IWatchListEndpoint _watchListEndpoint;

        public SeriesCollection SpySeriesCollection { get; set; }
        public List<string> SpyLabels { get; set; }

        public SeriesCollection DowSeriesCollection { get; set; }
        public List<string> DowLabels { get; set; }

        public DashboardViewModel(IStockDataEndpoint stockDataEndpoint, INewsEndpoint newsEndpoint, IWatchListEndpoint watchListEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _newsEndpoint = newsEndpoint;
            _watchListEndpoint = watchListEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadLeftChartData("aapl");
            await LoadRightChartData("^dji");
            await LoadWatchListData();
            await LoadMarketNews("amzn+aapl+wmt+fb");
        }

        private async Task LoadWatchListData()
        {
            
            var watchlist = await _watchListEndpoint.LoadWatchList();

            if(watchlist != null)
            {
                var watchlistData = new List<WatchlistDisplayModel>();

                string query = "";
                int index = 1;
                foreach (var stock in watchlist)
                {
                    if(index < watchlist.Count)
                    {
                        query = query + stock.Ticker + "%2C";
                    }
                    else
                    {
                        query = query + stock.Ticker;
                    }
                    
                    index++;
                }

                var stockData = await LoadMultipleStockData(query);

                WatchlistStocks = new BindingList<StockDashboardDataModel>(stockData);

                
            }

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
                var point = new OhlcPoint((double) result.Open, (double)result.High, (double)result.Low,(double) result.Close);
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
                var point = new OhlcPoint((double)result.Open, (double)result.High, (double)result.Low, (double)result.Close);
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
