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

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public DashboardViewModel(IStockDataEndpoint stockDataEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadSpyChartData();
        }

        private async Task LoadSpyChartData()
        {
            Labels = new List<string>();
            var Values = new ChartValues<OhlcPoint>();

            var results = await _stockDataEndpoint.GetSpyData("aapl", "2020-01-01", "2022-01-01", "daily");

            foreach(var result in results)
            {
                var point = new OhlcPoint(result.Open, result.High, result.Low, result.Close);
                Values.Add(point);
                Labels.Add(result.Date);
            }

            SeriesCollection = new SeriesCollection
            {
                new OhlcSeries()
                {
                    Values = Values
                }
            };

            NotifyOfPropertyChange(() => SeriesCollection);
            NotifyOfPropertyChange(() => Labels);
        }

        
    }
}
