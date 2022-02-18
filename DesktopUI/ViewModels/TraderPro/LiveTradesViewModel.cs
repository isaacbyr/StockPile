using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using WebSocket4Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DesktopUI.Library.Models;
using LiveCharts;
using LiveCharts.Defaults;
using DesktopUI.Library.Api.TraderPro;

namespace DesktopUI.ViewModels.TraderPro
{
    public class LiveTradesViewModel: Screen
    {
        WebSocket ws;
        private readonly IPolygonDataEndpoint _polygonDataEndpoint;

        public List<string> Labels { get; set; }
        public ChartValues<OhlcPoint> Values { get; set; }
        List<OhlcPoint> test = new List<OhlcPoint>();


        public LiveTradesViewModel(IPolygonDataEndpoint polygonDataEndpoint)
        {
           _polygonDataEndpoint = polygonDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadPreviousData();
            //StartConnection();
        }

        private async Task LoadPreviousData()
        {
            Labels = new List<string>();
            Values = new ChartValues<OhlcPoint>();

            var (opens, highs, lows, closes ,dates) = await _polygonDataEndpoint.LoadTradeData("AAPL", DateTime.Now.ToString("yyyy-MM-dd"));

            YMaxAxis = highs.Max() + 1;
            YMinAxis = lows.Min() - 1;
            XAxisMax = opens.Count + 10;

            for(int i = 0; i < opens.Count; i++)
            {
                var ohlc = new OhlcPoint(opens[i], highs[i], lows[i] ,closes[i]);
                Values.Add(ohlc);
                test.Add(ohlc);
                Labels.Add(dates[i].ToString("t"));
            }

            NotifyOfPropertyChange(() => Values);
            NotifyOfPropertyChange(() => Labels);
            NotifyOfPropertyChange(() => XAxisMax);

        }

        private void StartConnection()
        {
            ws = new WebSocket("wss://socket.polygon.io/stocks", sslProtocols: SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
            ws.Opened += ws_Opened;
            ws.Error += ws_Error;
            ws.Closed += ws_Closed;
            ws.MessageReceived += ws_MessageRecieved;
            ws.Open();

        }

        private void ws_MessageRecieved(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);

            var data = JArray.Parse(e.Message);

            if(IsConnecting)
            {
                foreach (JObject parsedObject in data.Children<JObject>())
                {
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        //string propertyName = parsedProperty.Name;
                        string propertyValue = (string)parsedProperty.Value;

                        if (propertyValue.Equals("success") && Count == 2)
                        {
                            IsConnecting = false;
                        }
                    }
                }
            }
            else
            {
                ConvertAndChart(data);
            }

            Count += 1;
        }

        private void ConvertAndChart(JArray data)
        {
            double open = 0;
            double high = 0;
            double low = 0;
            double close = 0;
            DateTime dt = DateTime.Now;

            foreach (JObject parsedObject in data.Children<JObject>())
            {
                foreach (JProperty parsedProperty in parsedObject.Properties())
                {
                    string propertyName = parsedProperty.Name;

                    if (propertyName.Equals("o"))
                    {
                        open = (double)parsedProperty.Value;
                    }
                    else if (propertyName.Equals("h"))
                    {
                        high = (double)parsedProperty.Value;
                    }
                    else if (propertyName.Equals("l"))
                    {
                        low = (double)parsedProperty.Value;
                    }
                    else if (propertyName.Equals("c"))
                    {
                        close = (double)parsedProperty.Value;
                    }
                    //else if (propertyName.Equals("s"))
                    //{
                    //    var ts = (long)parsedProperty.Value;
                    //    dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts).ToLocalTime();
                    //}
                }
            }

            var ohlc = new OhlcPoint(open, high, low ,close);

            Values.Add(ohlc);
            test.Add(ohlc);
        }

        private void ws_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection Closed...");
        }

        private void ws_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket Error");
            Console.WriteLine(e.Exception.Message);
        }

        private void ws_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Connected!");
            ws.Send("{\"action\":\"auth\",\"params\":\"g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1\"}");
            ws.Send("{\"action\":\"subscribe\",\"params\":\"A.AAPL\"}");
        }

        private bool _isConnecting = true;

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set 
            {
                _isConnecting = value;
                NotifyOfPropertyChange(() => IsConnecting);
            }
        }

        private int _count = 0;

        public int Count
        {
            get { return _count; }
            set 
            {
                _count = value;
                NotifyOfPropertyChange(() => Count);
            }
        }

        private double _yminAxis = 0;

        public double YMinAxis
        {
            get { return _yminAxis; }
            set
            {
                _yminAxis = value;
                NotifyOfPropertyChange(() => YMinAxis);
            }
        }

        private double _ymaxAxis = 100;

        public double YMaxAxis
        {
            get 
            {
                return _ymaxAxis;
            }
            set
            {
                _ymaxAxis = value;
                NotifyOfPropertyChange(() => YMaxAxis);
            }
        }

        private double _xAxisMax = 100;

        public double XAxisMax
        {
            get 
            {
                return _xAxisMax;
            }
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
