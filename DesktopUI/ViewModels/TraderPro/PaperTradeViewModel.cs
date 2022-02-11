using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Api.TraderPro;
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
        private readonly IPolygonDataEndpoint _polygonDataEndpoint;

        public ChartValues<OhlcPoint> Values { get; set; }

        public PaperTradeViewModel(IStockDataEndpoint stockDataEndpoint, IPolygonDataEndpoint polygonDataEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
            _polygonDataEndpoint = polygonDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            //await LoadData();
            //List<List<double>> temp = new List<List<double>>();
           //await LoadTest(temp);
        }


        private async Task LoadTest(List<List<double>> testValues)
        {
            int index = 0;
            int count = 0;

            var temp = new List<OhlcPoint>();
            //used to generate random values
            //List<List<double>> testValues = new List<List<double>>();
            //testValues.Add(new List<double> { 170, 171, 173, 169, 171 });
            //testValues.Add(new List<double> { 171, 172, 170, 171, 173 });
            //testValues.Add(new List<double> { 172, 173, 173, 173, 174 });
            //testValues.Add(new List<double> { 174, 173, 172, 171, 170, 171 });
            //testValues.Add(new List<double> { 171, 172, 174, 174, 175, 177 });
            //testValues.Add(new List<double> { 177, 177, 176, 176, 176, 175 });
            //testValues.Add(new List<double> { 175, 175, 176, 174, 175, 178 });

            double minValue = testValues[0].Min();
            double maxValue = testValues[0].Max();


            foreach (var l in testValues)
            {
                var tempMin = l.Min();
                if (tempMin < minValue)
                {
                    minValue = tempMin;
                }
                var tempMax = l.Max();
                if (tempMax > maxValue)
                {
                    maxValue = tempMax;
                }

            }

            MinAxisValue = minValue - 1;
            MaxAxisValue = maxValue + 1;
            AxisStep = 1;

            //AxisUnit = TimeSpan.TicksPerSecond;
            //AxisStep = TimeSpan.FromMilliseconds(1000).Ticks;

            //lets instead plot elapsed milliseconds and value
            var mapper = Mappers.Xy<MeasureModel>()
                .X(x => x.ElapsedMilliseconds)
                .Y(x => x.Value);


            //save the mapper globally         
            Charting.For<MeasureModel>(mapper);

            Values = new ChartValues<OhlcPoint>();


            var sw = new Stopwatch();
            sw.Start();
            var numCandle = 0;


            await Task.Run(() =>
            {

                while (numCandle < testValues.Count)
                {

                    Thread.Sleep(500);

                    if (count % 2 != 0)
                    {

                        //Thread.Sleep(500);
                        var point = new OhlcPoint();
                        double high = testValues[index - 1][0];
                        double low = testValues[index - 1][0];
                        double open = testValues[index - 1][0];

                        for (int j = 1; j < testValues[index - 1].Count; j++)
                        {
                            Thread.Sleep(50);
                            if (testValues[index - 1][j] >= testValues[index - 1][j - 1])
                            {
                                if (testValues[index - 1][j] > high)
                                {
                                    high = testValues[index - 1][j];
                                }
                                point = new OhlcPoint(open, high, low, testValues[index - 1][j]);
                            }
                            else
                            {
                                if (testValues[index - 1][j] < low)
                                {
                                    low = testValues[index - 1][j];
                                }
                                point = new OhlcPoint(open, high, low, testValues[index - 1][j]);
                            }

                            Values[index - 1] = point;
                            temp[index - 1] = point;
                            NotifyOfPropertyChange(() => Values);

                        }
                        count++;
                        numCandle++;
                    }
                    else
                    {
                        //we add the lecture based on our StopWatch instance
                        var point = new OhlcPoint(testValues[index][0], testValues[index][0], testValues[index][0], testValues[index][0]);
                        Values.Insert(index, point);
                        temp.Insert(index, point);
                        index++;
                        count++;
                        NotifyOfPropertyChange(() => Values);
                    }


                    //lets only use the last 150 values
                    if (Values.Count > 150)
                    {
                        Values.RemoveAt(0);
                    }
                }
            });
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

        private List<int> _chartInterval = new List<int> { 5, 10, 15 };

        public List<int> ChartInterval
        {
            get { return _chartInterval; }
            set { _chartInterval = value; }
        }

        private int _selectedChertInterval;

        public int SelectedChartInterval
        {
            get { return _selectedChertInterval; }
            set 
            {
                _selectedChertInterval = value;
                NotifyOfPropertyChange(() => SelectedChartInterval);
            }
        }


        private DateTime _selectedDate = DateTime.Now;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set 
            { 
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
            }
        }


        //private long _totalCount;

        //public long TotalCount
        //{
        //    get { return _totalCount; }
        //    set 
        //    {
        //        _totalCount = value;
        //        NotifyOfPropertyChange(() => TotalCount);
        //    }
        //}

        //private int _x1;

        //public int X1
        //{
        //    get { return _x1; }
        //    set
        //    {
        //        _x1 = value;
        //        NotifyOfPropertyChange(() => X1);
        //    }
        //}

        //private int _y1;

        //public int Y1
        //{
        //    get { return _y1; }
        //    set
        //    {
        //        _y1 = value;
        //        NotifyOfPropertyChange(() => Y1);
        //    }
        //}


        //private int _x2;

        //public int X2
        //{
        //    get { return _x2; }
        //    set 
        //    {
        //        _x2 = value;
        //        NotifyOfPropertyChange(() => X2);
        //    }
        //}

        //private int _y2;

        //public int Y2
        //{
        //    get { return _y2; }
        //    set 
        //    {
        //        _y2 = value;
        //        NotifyOfPropertyChange(() => Y2);
        //    }
        //}



        private double _axisStep;

        public double AxisStep
        {
            get { return _axisStep; }
            set
            {
                _axisStep = value;
                NotifyOfPropertyChange(() => AxisStep);
            }
        }


        private double _axisUnit;

        public double AxisUnit
        {
            get { return _axisUnit; }
            set
            {
                _axisUnit = value;
                NotifyOfPropertyChange(() => AxisUnit);
            }
        }

        public void SetAxisLimits(DateTime now)
        {
            XAxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            XAxisMin = now.Ticks - TimeSpan.FromSeconds(20).Ticks;

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

        private double _minAxisValue = 0;

        public double MinAxisValue
        {
            get { return _minAxisValue; }
            set 
            {
                _minAxisValue = value;
                NotifyOfPropertyChange(() => MinAxisValue);
            }
        }

        private double _maxAxisValue = 100;

        public double MaxAxisValue
        {
            get { return _maxAxisValue; }
            set 
            {
                _maxAxisValue = value;
                NotifyOfPropertyChange(() => MaxAxisValue);
            }
        }

        private double _xAxisMax;

        public double XAxisMax
        {
            get { return _xAxisMax; }
            set
            {
                _xAxisMax = value;
                NotifyOfPropertyChange(() => XAxisMax);
            }
        }

        private double _xAxisMin = 0;

        public double XAxisMin
        {
            get { return _xAxisMin; }
            set
            {
                _xAxisMin = value;
                NotifyOfPropertyChange(() => XAxisMin);
            }
        }

        public async Task SearchForTrades()
        {
            var (open, high, low, close) = await _polygonDataEndpoint.LoadTradeData("AAPL", "2022-02-09");

            var trades =  await GroupStockResults(open, high, low, close, 5);
            await LoadTest(trades);
        }

        private async Task<List<List<double>>> GroupStockResults(List<double> open, List<double> high, List<double> low, List<double> close, int interval)
        {
            int index = 0;
            var Trades = new List<List<double>>();

            var intervalList = new List<double>();
            while(index < open.Count)
            {
                for(int i = index; i < index + interval; i++)
                {
                    intervalList.Add(open[i]);
                    intervalList.Add(high[i]);
                    intervalList.Add(low[i]);
                    intervalList.Add(close[i]);
                }
                Trades.Add(intervalList);
                intervalList = new List<double>();

                index += interval; 
            }

            return Trades;
        }
    }

    
}
