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

        public SeriesCollection SpySeriesCollection { get; set; }
        public List<string> SpyLabels { get; set; }

        public SeriesCollection DowSeriesCollection { get; set; }
        public List<string> DowLabels { get; set; }

        public DashboardViewModel(IStockDataEndpoint stockDataEndpoint, INewsEndpoint newsEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _newsEndpoint = newsEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadSpyChartData();
            await LoadDowChartData();
            await LoadMarketNews("amzn+aapl+wmt+fb");
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

        private async Task LoadSpyChartData()
        {
            SpyLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var results = await _stockDataEndpoint.GetDashboardCharts("aapl");

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

        private async Task LoadDowChartData()
        {
            DowLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var results = await _stockDataEndpoint.GetDashboardCharts("^dji");

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

        private string _searchInput;

        public string SearchInput
        {
            get { return _searchInput; }
            set 
            {
                _searchInput = value;
                NotifyOfPropertyChange(() => SearchInput);
            }
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
