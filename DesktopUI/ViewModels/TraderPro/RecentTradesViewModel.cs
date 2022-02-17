using Caliburn.Micro;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.Models.TraderPro;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DesktopUI.ViewModels.TraderPro
{
    public class RecentTradesViewModel: Screen
    {
        public SeriesCollection SeriesCollection { get; set; }
        private readonly IPolygonDataEndpoint _polygonDataEndpoint;

        public RecentTradesViewModel(IPolygonDataEndpoint polygonDataEndpoint)
        {
           _polygonDataEndpoint = polygonDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            StartClock();
        }

         private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object sender, EventArgs e)
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

        private string _ticker;

        public string Ticker
        {
            get { return _ticker; }
            set
            {
                _ticker = value;
                NotifyOfPropertyChange(() => Ticker);
            }
        }

        private BindingList<RecentTradeModel> _recentTrades;

        public BindingList<RecentTradeModel> RecentTrades
        {
            get { return _recentTrades; }
            set 
            {
                _recentTrades = value;
                NotifyOfPropertyChange(() => RecentTrades);
            }
        }


        public async Task SearchForTrades()
        {
            var timestamp = ConvertDateToTimestamp();
            var trades = await _polygonDataEndpoint.LoadRecentTrades(Ticker, timestamp);
            RecentTrades = new BindingList<RecentTradeModel>(trades);
            if(RecentTrades != null)
            {
                LoadChart();
            }
        }

        private void LoadChart()
        {
            SeriesCollection = new SeriesCollection();
            var Values = new ChartValues<ScatterPoint>();

            foreach(var rt in RecentTrades)
            {
                var scatter = new ScatterPoint(rt.Timestamp, rt.Price, rt.Shares);
                Values.Add(scatter);
            }

            SeriesCollection.Add(
               new ScatterSeries
               {
                   Values = Values,
                   MinPointShapeDiameter = 15,
                   MaxPointShapeDiameter = 45
               });
            NotifyOfPropertyChange(() => SeriesCollection);
        }

        private double ConvertDateToTimestamp()
        {
            DateTime baseDate = new DateTime(1970, 01, 01);
            DateTime currDate;

            if (DateTime.Now.Hour < 13)
            {
                currDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            }
            else
            {
                currDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0);
            }

            var numberOfSeconds = currDate.Subtract(baseDate).TotalMilliseconds * 1000000;

            return numberOfSeconds;
        }
    }
}
