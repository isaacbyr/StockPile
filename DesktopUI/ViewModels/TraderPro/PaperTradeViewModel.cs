using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Models;
using DesktopUI.Library.Models.TraderPro;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.TraderPro
{
    public class PaperTradeViewModel: Screen
    {
        private readonly IStockDataEndpoint _stockDataEndpoint;

        public ChartValues<OhlcPoint> Values { get; set; }

        public PaperTradeViewModel(IStockDataEndpoint stockDataEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadData();
            await LoadChart();
        }

        private async Task LoadData()
        {
            var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts("AAPL", "5d", "15m");

            Stock = new BindingList<OhlcStockModel>(results);
        }

        private async Task LoadChart()
        {
            int idx = 0;

            //lets instead plot elapsed milliseconds and value
            var mapper = Mappers.Xy<MeasureModel>()
                .X(x => x.Session.Ticks)
                .Y(x => x.Value);

            //var mapper = Mappers.Financial<OhlcPoint>()
            //    .Open(x => (double)x.Open)
            //    .High(x => (double)x.High)
            //    .Low(x => (double)x.Low)
            //    .Close(x => (double)x.Close);

            //save the mapper globally         
            Charting.For<MeasureModel>(mapper);

            Values = new ChartValues<OhlcPoint>();

            var sw = new Stopwatch();
            sw.Start();

            await Task.Run(() =>
            {
                while (idx < Stock.Count)
                {
                    Thread.Sleep(1000);

                    var point = new OhlcPoint(Math.Round((double)Stock[idx].Open, 2), Math.Round((double)Stock[idx].High, 2), Math.Round((double)Stock[idx].Low, 2), Math.Round((double)Stock[idx].Close, 2));
                    Values.Add(point);
                    idx++;
                }
            });
        }

        private BindingList<OhlcStockModel> _stock;

        public BindingList<OhlcStockModel> Stock
        {
            get { return _stock; }
            set 
            {
                _stock = value;
                NotifyOfPropertyChange(() => Stock);
            }
        }

    }
}
