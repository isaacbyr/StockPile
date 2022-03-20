using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.Models.TraderPro;
using IBApi;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace DesktopUI.ViewModels.TraderPro
{
    public class TradeStrategyViewModel: Screen
    {
        WebSocket ws;
        private readonly ITWSTradingEndpoint _tWSTradingEndpoint;
        private readonly IStockDataEndpoint _stockDataEndpoint;

        public List<decimal> MA1History { get; set; }
        public List<decimal> MA2History { get; set; }


        public int ChartLength { get; set; } = 78;

        // Create the ibClient object to represent the connection
        EWrapperImpl ibClient;

        public TradeStrategyViewModel(bool addNew)
        {
            AddNew = addNew;

            // instanitate the ibClient
            ibClient = new EWrapperImpl();
        }

        public TradeStrategyViewModel(string ticker, int buyShares, int sellShares, string ma1, string ma2, 
            string indicator, string interval, string range, bool addNew ,
            ITWSTradingEndpoint tWSTradingEndpoint, IStockDataEndpoint stockDataEndpoint)
        {
            Ticker = ticker;
            BuyShares = buyShares;
            SellShares = sellShares;
            MA1 = ma1;
            MA2 = ma2;
            Indicator = indicator;
            Interval = interval;
            AddNew = addNew;
            Range = range;
            _tWSTradingEndpoint = tWSTradingEndpoint;
            _stockDataEndpoint = stockDataEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            if(AddNew == true)
            {
                await PostTWSTradeStrategy();

            }

            await LoadTWSStrategies();
            var results1 =  await LoadEMAHistory(ActiveMA1);
            MA1History = new List<decimal>(results1);
            var results2 = await LoadEMAHistory(ActiveMA2);
            MA2History = new List<decimal>(results2);
            CurrentMA1 = await UpdateEMA(results1, ActiveMA1);
            CurrentMA2 = await UpdateEMA(results2, ActiveMA2);

            if(CurrentMA1 > CurrentMA2)
            {
                IsTop = true;
            }
            else
            {
                IsTop = false;
            }
            StartConnection();

        }

        // start webstocket connection
        private void StartConnection()
        {
            ws = new WebSocket("wss://socket.polygon.io/stocks", sslProtocols: SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls);
            ws.Opened += ws_Opened;
            ws.Error += ws_Error;
            ws.Closed += ws_Closed;
            ws.MessageReceived += ws_MessageRecieved;
            ws.Open();

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
            string ticker = "NFLX";

            Console.WriteLine("Connected!");
            ws.Send("{\"action\":\"auth\",\"params\":\"g3B6V1o8p6eb1foQLIPYHI46hrnq8Sw1\"}");
            //ws.Send("{\"action\":\"subscribe\",\"params\":\"A.AAPL\"}");

            var wsModel = new WSModel
            {
                Action = "subscribe",
                Params = $"A.{ticker}"
            };

            ws.Send(wsModel.ToString());

        }


        private void ws_MessageRecieved(object sender, MessageReceivedEventArgs e)
        {

            var data = JArray.Parse(e.Message);

            if (IsConnecting)
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
                ConvertWSData(data);
            }

            Count += 1;
        }

        // once we start receiving stock data from ws call this function to parse data
        private async Task ConvertWSData(JArray data)
        {
            DateTime dt = DateTime.Now;
            decimal close = 0;

            foreach (JObject parsedObject in data.Children<JObject>())
            {
                foreach(JProperty parsedProperty in parsedObject.Properties())
                {
                    string propertyName = parsedProperty.Name;

                     if (propertyName.Equals("c"))
                    {
                        close = (decimal)parsedProperty.Value;
                        CurrentPrice = (double)Math.Round(close, 2);
                    }
                    else if (propertyName.Equals("s"))
                    {
                        var ts = (long)parsedProperty.Value;
                        dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(ts).ToLocalTime();
                    }
                }
            }

            int activeInterval = ConvertInterval(ActiveInterval);

            if (dt.Minute % activeInterval == 0 && dt.Second == 0)
            {
                MA1History.Add(close);
                MA2History.Add(close);
                CurrentMA1 = await UpdateEMA(MA1History, ActiveInterval);
                CurrentMA2 = await UpdateEMA(MA2History, ActiveInterval);
                await FindCrosovers(CurrentMA1, CurrentMA2);
            }
            else
            {
                MA1History[MA1History.Count - 1] = close;
                MA2History[MA2History.Count - 1] = close;
                CurrentMA1 = await UpdateEMA(MA1History, ActiveInterval);
                CurrentMA2 = await UpdateEMA(MA2History, ActiveInterval);
                await FindCrosovers(CurrentMA1, CurrentMA2);
            }
        }


        // start TWS Connection
        public void Connect()
        {
            // Parameters to connect to TWS
            // host - IP address or host name of the host running TWS
            // port - listening to port 7497
            // clientID - client application identifier - can be any number
            ibClient.ClientSocket.eConnect("", 7497, 0);

            var reader = new EReader(ibClient.ClientSocket, ibClient.Signal);
            reader.Start();
            new Thread(() => {
                while (ibClient.ClientSocket.IsConnected())
                {
                    ibClient.Signal.waitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }.Start();

            while (ibClient.NextOrderId <= 0) 
            {
                getData();
            }

            // load in order id 
            OrderId = ibClient.NextOrderId;

            //ibClient.ibVM = this;

        }

        // fucnction to request market data from IBAPI
        private void getData()
        {
            ibClient.ClientSocket.cancelMktData(1); // cancel market data

            // create a new contract to specuft the security we are searching for
            var contract = new Contract();

            // Create a new TagValueList Object 
            List<TagValue> mktDataOptions = new List<TagValue>();

            // Set the stock ticker to get data for
            contract.Symbol = Ticker;
  
            // Set the security type to stk for a stock
            contract.SecType = "STK";
            // Use "SMART" as the general exchange;
            contract.Exchange = "SMART";
            // Set Exchange
            contract.PrimaryExch = "NYSE";
            // Set currency
            contract.Currency = "USD";

            // request to use delayed data
            ibClient.ClientSocket.reqMarketDataType(3); // 3

            ibClient.ClientSocket.reqMktData(1, contract, "233", false, false, mktDataOptions);
        }

        // this funciton updates ema values as new prices come in from websocket
        private async Task<decimal> UpdateEMA(List<decimal> results, string emaInterval)
        {
            int emaRange;
            int.TryParse(emaInterval, out emaRange);

            decimal k = (decimal)2 / (emaRange + 1);
            decimal sum = 0;

            var maList = new List<decimal>();

            for (int i = 0; i < emaRange; i++)
            {
                sum += results[i];
            }
            decimal sma = sum / emaRange;
            maList.Add(sma);

            int index = 0;
            for (int i = emaRange; i < results.Count; i++)
            {
                decimal ema = (results[i] * k) + (maList[index] * (1 - k));
                maList.Add(ema);

                index += 1;
            }

            var removeValues = maList.Count - ChartLength;
            int idx = 0;
            while (idx < removeValues)
            {
                maList.RemoveAt(0);
                idx++;
            }

            return Math.Round(maList[maList.Count-1],2);

        }

        // this function loads in all the previous data for the ema 
        public async Task<List<decimal>> LoadEMAHistory(string emaInterval)
        {
            int emaRange;
            int.TryParse(emaInterval, out emaRange);


            if (ActiveMA1 != null && ActiveInterval != null)
            {
                var (range, lastResults) = AddAndConvertDays(ActiveRange, ActiveInterval, emaRange);

                var (results, symbol, marketPrice) = await _stockDataEndpoint.GetMAChartData(ActiveTicker, range, ActiveInterval, lastResults);

                List<decimal> closes = new List<decimal>();

                 foreach(var r in results)
                  {
                    closes.Add(r.Close);
                  }

                return closes;
            }
            else
            {
                return new List<decimal> { 0 };
            }
  
        }

        public async Task FindCrosovers(decimal currentMA1, decimal currentMA2)
        {
           
            if(IsTop)
            {
                // represents a sell
                if (currentMA1 <= currentMA2)
                {
                    send_order("BUY");
                }
            }
            else
            {
                // represents a buy
                if(currentMA1 >= currentMA2)
                {
                    send_order("SELL");
                }
            }
        }

        public void send_order(string side)
        {
            // create a new contract to specify the security we are looking for
            var contract = new Contract();

            contract.Symbol = Ticker;
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.PrimaryExch = "ISLAND";
            contract.Currency = "USD";

            var order = new Order();
            // Set OrderId
            order.OrderId = OrderId;
            // get the side of the order (buy or sell)
            order.Action = side;
            // get thes order type (limit or market)
            order.OrderType = "MKT";
            // gets number of shares (double)
            order.TotalQuantity = (side == "BUY" ? BuyShares : SellShares);
            // gets price converts to double
            order.LmtPrice = CurrentPrice;

            // visible shares to the market
            order.DisplaySize = 100;
            // is outside trading hours
            order.OutsideRth = true;

            //place the order
            ibClient.ClientSocket.placeOrder(OrderId, contract, order);

            OrderId += 2;

        }


        // return interval and string
        public (string, int) AddAndConvertDays(string range, string interval, int smaRange)
        {
            decimal temp;

            if (ActiveRange == "1d" || ActiveRange == "5d")
            {
                decimal days;
                decimal.TryParse(ActiveRange.Substring(0, 1), out days);

                var denom = Math.Ceiling(ChartLength / days);
                temp = Math.Ceiling((ChartLength + (decimal)smaRange) / denom);

                return ($"{temp}d", ChartLength + smaRange);
            }
            else if (ActiveRange == "1mo" && ActiveInterval == "5m")
            {

                temp = Math.Ceiling((ChartLength + (decimal)smaRange) / 79);

                return ($"{temp}d", ChartLength + smaRange);
            }
            else if (ActiveRange == "1mo" && ActiveInterval == "15m")
            {

                temp = Math.Ceiling((ChartLength + (decimal)smaRange) / 27);

                return ($"{temp}d", ChartLength + smaRange);
            }
            else if (ActiveRange == "1mo" && ActiveInterval == "1d")
            {
                temp = (ChartLength + smaRange);
                return ($"{temp}d", ChartLength + smaRange);
            }
            else if (ActiveRange == "3mo" || ActiveRange == "6mo")
            {
                temp = (ChartLength + smaRange);
                return ($"{temp}d", ChartLength + smaRange);

            }
            //TODO SET THIS UP PROPERLY FOR 1YR AND 5YR
            return ("", 0);
        }

        private async Task LoadTWSStrategies()
        {
            var results = await _tWSTradingEndpoint.LoadStrategies();
            Strategies = new BindingList<TWSTradeModel>(results);
        }

        private async Task PostTWSTradeStrategy()
        {
            var trade = new TWSTradeModel
            {
                Ticker = Ticker,
                BuyShares = BuyShares,
                SellShares = SellShares,
                MA1 = MA1,
                MA2 = MA2,
                Indicator = Indicator,
                Interval = Interval,
                Range = Range,
            };

            await _tWSTradingEndpoint.PostTWSStategy(trade);
        }

        private int ConvertInterval(string interval)
        {
            switch (interval)
            {
                case "5m":
                    return 5;
                case "10m":
                    return 10;
                case "15m":
                    return 15;
                default:
                    return 1;
            }
        }

        private bool _addNew;

        public bool AddNew
        {
            get { return _addNew; }
            set 
            {
                _addNew = value;
                NotifyOfPropertyChange(() => AddNew);
            }
        }

        private BindingList<TWSTradeModel> _strategies;

        public BindingList<TWSTradeModel> Strategies
        {
            get { return _strategies; }
            set 
            {
                _strategies = value;
                NotifyOfPropertyChange(() => Strategies);
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

        private int _buyShares;

        public int BuyShares
        {
            get { return _buyShares; }
            set 
            {
                _buyShares = value;
                NotifyOfPropertyChange(() => BuyShares);
            }
        }

        private int _sellShares;

        public int SellShares
        {
            get { return _sellShares; }
            set 
            {
                _sellShares = value;
                NotifyOfPropertyChange(() => SellShares);
            }
        }

        private string _ma1;

        public string MA1
        {
            get { return _ma1; }
            set 
            {
                _ma1 = value;
                NotifyOfPropertyChange(() => MA1);
            }
        }

        private string _ma2;

        public string MA2
        {
            get { return _ma2; }
            set 
            {
                _ma2 = value;
                NotifyOfPropertyChange(() => MA2);
            }
        }

        private string _interval;

        public string Interval
        {
            get { return _interval; }
            set 
            {
                _interval = value;
                NotifyOfPropertyChange(() => Interval);
            }
        }

        private string _indicator;

        public string Indicator
        {
            get { return _indicator; }
            set 
            {
                _indicator = value;
                NotifyOfPropertyChange(() => Indicator);
            }
        }

        private string _range;

        public string Range
        {
            get { return _range; }
            set 
            {
                _range = value;
                NotifyOfPropertyChange(() => Range);
            }
        }

        private double _currentPrice;

        public double CurrentPrice
        {
            get { return _currentPrice; }
            set 
            {
                _currentPrice = value;
                NotifyOfPropertyChange(() => CurrentPrice);
            }
        }

        private decimal _currentMA1;

        public decimal CurrentMA1
        {
            get { return _currentMA1; }
            set 
            {
                _currentMA1 = value;
                NotifyOfPropertyChange(() => CurrentMA1);
            }
        }

        private decimal _currentMA2;

        public decimal CurrentMA2
        {
            get { return _currentMA2; }
            set
            {
                _currentMA2 = value;
                NotifyOfPropertyChange(() => CurrentMA2);
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

        private string _activeMA1 = "13";

        public string ActiveMA1
        {
            get { return _activeMA1; }
            set 
            {
                _activeMA1 = value;
                NotifyOfPropertyChange(() => ActiveMA1);
            }
        }

        private string _activeMA2 = "21";

        public string ActiveMA2
        {
            get { return _activeMA2; }
            set 
            {
                _activeMA2 = value;
                NotifyOfPropertyChange(() => ActiveMA2);
            }
        }

        private string _activeIndicator = "EMA";

        public string ActiveIndicator
        {
            get { return _activeIndicator; }
            set 
            {
                _activeIndicator = value;
                NotifyOfPropertyChange(() => ActiveIndicator);
            }
        }

        private string _activeRange = "1mo";

        public string ActiveRange
        {
            get { return _activeRange; }
            set 
            {
                _activeRange = value;
                NotifyOfPropertyChange(() => ActiveRange);
            }
        }

        private string _activeInterval = "5m";

        public string ActiveInterval
        {
            get { return _activeInterval; }
            set 
            {
                _activeInterval = value;
                NotifyOfPropertyChange(() => ActiveInterval);
            }
        }

        private string _activeTicker = "AAPL";

        public string ActiveTicker
        {
            get { return _activeTicker; }
            set 
            {
                _activeTicker = value;
                NotifyOfPropertyChange(() => ActiveTicker);
            }
        }

        private bool _isTop;

        public bool IsTop
        {
            get { return _isTop; }
            set 
            {
                _isTop = value;
                NotifyOfPropertyChange(() => IsTop);
            }
        }

        private int _orderId = 0;

        public int OrderId
        {
            get { return _orderId; }
            set
            {
                _orderId = value;
                NotifyOfPropertyChange(() => OrderId);
            }
        }

    }
}
