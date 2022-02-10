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

        //public ChartValues<MeasureModel> Values { get; set; }
        public ChartValues<OhlcPoint> Values { get; set; }

        public PaperTradeViewModel(IStockDataEndpoint stockDataEndpoint)
        {
            _stockDataEndpoint = stockDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            //await LoadData();
            //LoadChart();
            LoadTest();
            
            
        }


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


        public long startTimeTicks { get; set; }


        //private void LoadTester()
        //{
        //    int index = 0;
        //    int count = 0;
        //    //used to generate random values
        //    List<DateTime> xvalues = new List<DateTime>();
        //    List<List<double>> testValues = new List<List<double>>();
        //    testValues.Add(new List<double> { 170, 171, 173, 169, 171 });
        //    testValues.Add(new List<double> { 171, 172, 170, 171, 173 });
        //    testValues.Add(new List<double> { 172, 173, 173, 173, 174 });
        //    testValues.Add(new List<double> { 174, 173, 172, 171, 170, 171 });

        //    double minValue = testValues[0].Min();
        //    double maxValue = testValues[0].Max();


        //    foreach (var l in testValues)
        //    {
        //        var tempMin = l.Min();
        //        if (tempMin < minValue)
        //        {
        //            minValue = tempMin;
        //        }
        //        var tempMax = l.Max();
        //        if (tempMax > maxValue)
        //        {
        //            maxValue = tempMax;
        //        }

        //    }

        //    MinAxisValue = minValue - 10;
        //    MaxAxisValue = maxValue + 10;
        //    //lets instead plot elapsed milliseconds and value
        //    var mapper = Mappers.Financial<MeasureModel>()
        //        .X(x => x.DateTime.Ticks)
        //        .Open(x => x.value.Open)
        //        .High(x => x.value.High)
        //        .Low(x => x.value.Low)
        //        .Close(x => x.value.Close);

        //    AxisStep = TimeSpan.FromSeconds(5).Ticks;
        //    AxisUnit = TimeSpan.TicksPerSecond;

            
        //    //save the mapper globally         
        //    Charting.For<MeasureModel>(mapper);

        //    Values = new ChartValues<MeasureModel>();

        //    var numCandle = 0;
        //    Task.Run(() =>
        //    {

        //        var currentTime = DateTime.Now;
        //        startTimeTicks = currentTime.Ticks; // store start time
        //        SetAxisLimits(currentTime);

        //        while (numCandle < testValues.Count)
        //        {

                    
        //            Thread.Sleep(1000);
        //            var now = DateTime.Now;
        //            //SetAxisLimits(now);

        //            if (count % 2 != 0)
        //            {

        //                //Thread.Sleep(500);
        //                var point = new OhlcPoint();
        //                double high = testValues[index - 1][0];
        //                double low = testValues[index - 1][0];
        //                double open = testValues[index - 1][0];

        //                for (int j = 1; j < testValues[index - 1].Count; j++)
        //                {
        //                    Thread.Sleep(1000);
        //                    if (testValues[index - 1][j] >= testValues[index - 1][j - 1])
        //                    {
        //                        if (testValues[index - 1][j] > high)
        //                        {
        //                            high = testValues[index - 1][j];
        //                        }
        //                        point = new OhlcPoint(open, high, low, testValues[index - 1][j]);
        //                    }
        //                    else
        //                    {
        //                        if (testValues[index - 1][j] < low)
        //                        {
        //                            low = testValues[index - 1][j];
        //                        }
        //                        point = new OhlcPoint(open, high, low, testValues[index - 1][j]);
        //                    }

        //                    var measure = new MeasureModel
        //                    {
        //                        DateTime = xvalues[index-1],
        //                        value = point
        //                    };
        //                    Values[index - 1] = measure;

        //                }
        //                count++;
        //                numCandle++;
        //            }
        //            else
        //            {
        //                //we add the lecture based on our StopWatch instance
        //                var point = new OhlcPoint(testValues[index][0], testValues[index][0], testValues[index][0], testValues[index][0]);
        //                Values.Insert(index, new MeasureModel
        //                {
        //                    DateTime = now - new TimeSpan(startTimeTicks),
        //                    value = point
        //                });
        //                index++;
        //                count++;
        //            }

        //            xvalues.Add(now - new TimeSpan(startTimeTicks));
        //            SetAxisLimits(now - new TimeSpan(startTimeTicks));

        //            //lets only use the last 150 values
        //            if (Values.Count > 150)
        //            {
        //                Values.RemoveAt(0);
        //            }
        //        }
        //    });
        //}

        public void SetAxisLimits(DateTime now)
        {
            XAxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 1 second ahead
            XAxisMin = now.Ticks - TimeSpan.FromSeconds(20).Ticks;
            //XAxisMax = now.Ticks + TimeSpan.FromSeconds(5).Ticks; // lets force the axis to be 1 second ahead
            //XAxisMin = now.Ticks - TimeSpan.FromSeconds(5).Ticks > 0 ? now.Ticks - TimeSpan.FromSeconds(5).Ticks : 0; // and 20 seconds behind
        }

        //public class MeasureModel
        //{
        //    public DateTime DateTime { get; set; }
        //    public OhlcPoint value { get; set; }
        //}

        private void LoadTest()
        {
            int index = 0;
            int count = 0;
            //used to generate random values
            List<List<double>> testValues = new List<List<double>>();
            testValues.Add(new List<double> { 170, 171, 173, 169, 171 });
            testValues.Add(new List<double> { 171, 172, 170, 171, 173 });
            testValues.Add(new List<double> { 172, 173, 173, 173, 174 });
            testValues.Add(new List<double> { 174, 173, 172, 171, 170, 171 });
            testValues.Add(new List<double> { 171, 172, 174, 174, 175, 177 });
            testValues.Add(new List<double> { 177, 177, 176, 176, 176, 175 });
            testValues.Add(new List<double> { 175, 175, 176, 174, 175, 178 });




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

            MinAxisValue = minValue - 10;
            MaxAxisValue = maxValue + 10;
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


            Task.Run(() =>
            {

                while (numCandle < testValues.Count)
                {

                    Thread.Sleep(1000);

                    if (count % 2 != 0)
                    {

                        //Thread.Sleep(500);
                        var point = new OhlcPoint();
                        double high = testValues[index - 1][0];
                        double low = testValues[index - 1][0];
                        double open = testValues[index - 1][0];

                        for (int j = 1; j < testValues[index - 1].Count; j++)
                        {
                            Thread.Sleep(1000);
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

                        }
                        count++;
                        numCandle++;
                    }
                    else
                    {
                        //we add the lecture based on our StopWatch instance
                        var point = new OhlcPoint(testValues[index][0], testValues[index][0], testValues[index][0], testValues[index][0]);
                        Values.Insert(index, point);
                        index++;
                        count++;
                    }


                    //lets only use the last 150 values
                    if (Values.Count > 150)
                    {
                        Values.RemoveAt(0);
                    }
                }
            });
        }



        //private async Task LoadData()
        //{
        //    var (results, symbol, marketPrice) = await _stockDataEndpoint.GetDashboardCharts("AAPL", "5d", "15m");

        //    Stock = new BindingList<OhlcStockModel>(results);
        //}

        //private void LoadChart()
        //{
        //    int idx = 0;

        //    //lets instead plot elapsed milliseconds and value
        //    var mapper = Mappers.Xy<MeasureModel>()
        //        .X(x => x.Session.Ticks)
        //        .Y(x => x.Value);

        //    //save the mapper globally         
        //    Charting.For<MeasureModel>(mapper);

        //    Values = new ChartValues<OhlcPoint>();

        //    var sw = new Stopwatch();
        //    sw.Start();

        //    Task.Run(() =>
        //    {
        //        while (idx < Stock.Count)
        //        {
        //            Thread.Sleep(1000);

        //            var point = new OhlcPoint(Math.Round((double)Stock[idx].Open, 2), Math.Round((double)Stock[idx].High, 2), Math.Round((double)Stock[idx].Low, 2), Math.Round((double)Stock[idx].Close, 2));
        //            Values.Add(point);
        //            idx++;
        //        }
        //    });
        //}

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

        private double _minAxisValue;

        public double MinAxisValue
        {
            get { return _minAxisValue; }
            set 
            {
                _minAxisValue = value;
                NotifyOfPropertyChange(() => MinAxisValue);
            }
        }

        private double _maxAxisValue;

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



    }

    
}
