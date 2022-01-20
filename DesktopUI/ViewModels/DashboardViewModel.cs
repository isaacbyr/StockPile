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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels
{
    public class DashboardViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;

        public SeriesCollection SpySeriesCollection { get; set; }
        public List<string> SpyLabels { get; set; }

        public SeriesCollection DowSeriesCollection { get; set; }
        public List<string> DowLabels { get; set; }

        public DashboardViewModel(IStockDataEndpoint stockDataEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            // await LoadSpyChartData();
            //await LoadDowChartData();
            await LoadSpyIntraData();
            await LoadDowIntraData();
        }

        private async Task LoadDowIntraData()
        {
            DowLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var results = await _stockDataEndpoint.GetSpyIntra("^dji");

            foreach (var result in results)
            {
                var point = new OhlcPoint((double)result.Open, (double)result.High, (double)result.Low, (double)result.Close);
                Values.Add(point);
                //SpyLabels.Add(result.Date);
            }

            DowSeriesCollection = new SeriesCollection
            {
                new OhlcSeries()
                {
                    Values = Values
                }
            };

            NotifyOfPropertyChange(() => DowSeriesCollection);
            NotifyOfPropertyChange(() => DowLabels);
        }

        private async Task LoadSpyIntraData()
        {
            SpyLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var results = await _stockDataEndpoint.GetSpyIntra("spy");

            foreach (var result in results)
            {
                var point = new OhlcPoint((double) result.Open, (double)result.High, (double)result.Low,(double) result.Close);
                Values.Add(point);
                //SpyLabels.Add(result.Date);
            }

            SpySeriesCollection = new SeriesCollection
            {
                new OhlcSeries()
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

            var results = await _stockDataEndpoint.GetDowData("^dji");

            foreach (var result in results)
            {
                var point = new OhlcPoint(result.Open, result.High, result.Low, result.Close);
                Values.Add(point);
                DowLabels.Add(result.Date);
            }

            DowSeriesCollection = new SeriesCollection
            {
                new OhlcSeries()
                {
                    Values = Values
                }
            };

            NotifyOfPropertyChange(() => DowSeriesCollection);
            NotifyOfPropertyChange(() => DowLabels);
        }

        private async Task LoadSpyChartData()
        {
            SpyLabels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var results = await _stockDataEndpoint.GetSpyData("spy", "2020-01-01", "2022-01-01", "daily");

            foreach(var result in results)
            {
                var point = new OhlcPoint(result.Open, result.High, result.Low, result.Close);
                Values.Add(point);
                SpyLabels.Add(result.Date);
            }

            SpySeriesCollection = new SeriesCollection
            {
                new OhlcSeries()
                {
                    Values = Values
                }
            };

            NotifyOfPropertyChange(() => SpySeriesCollection);
            NotifyOfPropertyChange(() => SpyLabels);
        }

        
    }
}
