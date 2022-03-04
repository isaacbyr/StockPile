using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using IBApi;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using System.ComponentModel;
using DesktopUI.Library.Models.TraderPro;

namespace DesktopUI.ViewModels.TraderPro
{
    public class IBViewModel: Screen
    {
        // Create the ibClient object to represent the connection
        EWrapperImpl ibClient;

        public IBViewModel()
        {
            // instanitate the ibClient
            ibClient = new EWrapperImpl();

        }

        protected override async void OnViewLoaded(object view)
        {
            
        }

        private void getData()
        {
           TimeAndSales = new AsyncObservableCollection<TimeAndSalesModel>();

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

        public void GetTickerPrice(string _tickerPrice)
        {
            string[] tickerPrice = new string[] { _tickerPrice };
            tickerPrice = _tickerPrice.Split(',');

            if (Convert.ToInt32(tickerPrice[0]) == 1)
            {
                if (Convert.ToInt32(tickerPrice[1]) == 68) // 68
                {
                    // LAST QUOTE FOR DELAYED DATA
                    Console.WriteLine("DELAYED QUOTE: ", tickerPrice[2]);
                    Last = Convert.ToDouble(tickerPrice[2]);
                }
                else if (Convert.ToInt32(tickerPrice[1]) == 67) //  67
                {
                    // DELAYED ASK QUOTE
                    Console.WriteLine("DELAYED ASK QUOTE: ", tickerPrice[2]);
                    Ask = Convert.ToDouble(tickerPrice[2]);
                }
                else if (Convert.ToInt32(tickerPrice[1]) == 66) // 66
                {
                    // DELAYED BIG QUOTE
                    Console.WriteLine("DELAYED BID QUOTE", tickerPrice[2]);
                    Bid = Convert.ToDouble(tickerPrice[2]);
                }
            }


        }

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

            while (ibClient.NextOrderId <= 0) { }

            ibClient.ibVM = this;

            // load in order id 
            OrderId = ibClient.NextOrderId;

            Status = "Connected :)";
            getData();
            StartTimer();
        }

        public void AddTickString(string _tickString)
        {
            try
            {
                // extract each value from string and store in a string array
                string[] listTimeSales = _tickString.Split(';');

                // get values from array and convert to double
                double lastPrice = Convert.ToDouble(listTimeSales[0]);
                decimal shares;
                decimal.TryParse(listTimeSales[1], out shares);
                int tradeSize = (int)Math.Floor(shares);
                double tradeTime = Convert.ToDouble(listTimeSales[2]);

                // add two zeros to trade size
                int shareSize = tradeSize * 100;
                string strShareSize = shareSize.ToString("###,####,##0");

                // convert date
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epoch = epoch.AddMilliseconds(tradeTime);
                epoch = epoch.AddHours(-8);

                // get mean price
                double meanPrice = ((Ask - Bid) / 2);
                double mean = Bid + meanPrice;

                var timeAndSales = new TimeAndSalesModel();

                if(lastPrice >= Ask)
                {
                    timeAndSales.Price = lastPrice;
                    timeAndSales.ShareSize = strShareSize;
                    timeAndSales.Time = epoch.ToString("h:mm:ss:ff");
                    timeAndSales.Condition = "OnAsk";
                }
                else if (lastPrice <= Bid)
                {
                    timeAndSales.Price = lastPrice;
                    timeAndSales.ShareSize = strShareSize;
                    timeAndSales.Time = epoch.ToString("h:mm:ss:ff");
                    timeAndSales.Condition = "OnBid";
                }
                else if(lastPrice > mean && lastPrice < Ask)
                {
                    timeAndSales.Price = lastPrice;
                    timeAndSales.ShareSize = strShareSize;
                    timeAndSales.Time = epoch.ToString("h:mm:ss:ff");
                    timeAndSales.Condition = "UnderAsk";
                }
                else
                {
                    // last price under the mean price but greater than bid
                    timeAndSales.Price = lastPrice;
                    timeAndSales.ShareSize = strShareSize;
                    timeAndSales.Time = epoch.ToString("h:mm:ss:ff");
                    timeAndSales.Condition = "UnderAsk";
                }

                // TODO: ADD MORE CONDTIONS AND CREATE COLOR CONVERTER
                TimeAndSales.Add(timeAndSales);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Disconnect()
        {
            ibClient.ClientSocket.eDisconnect();
            Status = "Disconnected :(";
        }

        public void Buy()
       {
            string side = "BUY";
            send_order(side);
        }
        
        public void BracketBuy()
        {
            string side = "BUY";
            send_bracket_order(side);
        }

        public void BracketSell()
        {
            string side = "SELL";
            send_bracket_order(side);
        }

        public void Sell()
        {
            string side = "SELL";
            send_order(side);
        }

        public void send_bracket_order(string side)
        {
            // create a new contract
            var contract = new Contract();
            // set underlining values for a contract
            contract.Symbol = Ticker;
            contract.SecType = "STK";
            contract.Exchange = SelectedMarket;
            contract.PrimaryExch = "ISLAND";
            contract.Currency = "USD";

            // gets the next order id from the text box
            int orderId = OrderId;
            // get the side of the order (buy or sell)
            string action = side;
            // get thes order type (limit or market)
            string order_type = SelectedType;
            // gets number of shares (double)
            int quantity = Quantity;
            // gets price 
            double lmtPrice = LMTPrice;
            // gets take profit amount
            double takeProfit = TakeProfit;
            // gets stop loss 
            double stopLoss = StopLoss;

            // TODO: INSTEAD OF HARDCODING STOP LOSS WE CAN ADD IN A CALCULATION LIKE 50 CENTS OR 5% ETC;


            // calls a bracket order function and stores the results in a list variable called bracket
            List<Order> bracket = BracketOrder(OrderId, action, quantity, lmtPrice, takeProfit, stopLoss, order_type);

            // bracket order function returns list of all the strings each one representing an order;
            foreach(Order o in bracket)
            {
                ibClient.ClientSocket.placeOrder(o.OrderId, contract, o);
            }

            // increment order id by 3 to reduce chance or repeating
            OrderId += 3;
        }

        public static List<Order> BracketOrder(int parentOrderId, string action, double quantity, double limitPrice, double takeProfitLimitPrice, double stopLossPrice, string order_type)
        {
            // main parent order
            Order parent = new Order();
            parent.ParentId = parentOrderId;
            parent.Action = action; // buy or sell
            parent.OrderType = order_type;
            parent.TotalQuantity = quantity;
            parent.LmtPrice = limitPrice;
            // parent and children orders will need this attribute set to false to prevent accidental executions
            // the last child will have it set to true
            parent.Transmit = false;

            // Profit Target Order
            Order takeProfit = new Order();
            takeProfit.OrderId = parentOrderId + 1;
            takeProfit.Action = action.Equals("BUY") ? "SELL" : "BUY";
            takeProfit.OrderType = "LMT";
            takeProfit.TotalQuantity = quantity;
            // take profit price
            takeProfit.LmtPrice = takeProfitLimitPrice;
            takeProfit.ParentId = parentOrderId;
            takeProfit.Transmit = false;

            // stop loss order
            Order stopLoss = new Order();
            stopLoss.OrderId = parentOrderId + 2;
            takeProfit.Action = action.Equals("BUY") ? "SELL" : "BUY";
            stopLoss.OrderType = "STP";
            // stop trigger price
            stopLoss.AuxPrice = stopLossPrice;
            stopLoss.TotalQuantity = quantity;
            stopLoss.ParentId = parentOrderId;
            stopLoss.Transmit = true;

            List<Order> bracketOrder = new List<Order>();
            bracketOrder.Add(parent);
            bracketOrder.Add(takeProfit);
            bracketOrder.Add(stopLoss);

            return bracketOrder;
        }

        public void send_order(string side)
        {
            // create a new contract to specify the security we are looking for
            var contract = new Contract();

            contract.Symbol = Ticker;
            contract.SecType = "STK";
            contract.Exchange = SelectedMarket;
            contract.PrimaryExch = "ISLAND";
            contract.Currency = "USD";

            var order = new Order();
            // gets the next order id from the text box
            order.OrderId = OrderId;
            // get the side of the order (buy or sell)
            order.Action = side;
            // get thes order type (limit or market)
            order.OrderType = SelectedType;
            // gets number of shares (double)
            order.TotalQuantity = Quantity;
            // gets price converts to double
            order.LmtPrice = LMTPrice;

            // checks for a stop order
            if (SelectedType == "STP")
            {
                order.AuxPrice = LMTPrice;
            }
            // visible shares to the market
            order.DisplaySize = Visible;
            // is outside trading hours
            order.OutsideRth = true;

            //place the order
            ibClient.ClientSocket.placeOrder(OrderId, contract, order);

            OrderId++;


        }

        private AsyncObservableCollection<TimeAndSalesModel> _timeAndSales;

        public AsyncObservableCollection<TimeAndSalesModel> TimeAndSales
        {
            get { return _timeAndSales; }
            set 
            {
                _timeAndSales = value;
                NotifyOfPropertyChange(() => TimeAndSales);
            }
        }


        private string _status = "Disconnected :(";

        public string Status
        {
            get { return _status; }
            set 
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        private double _last = 0.0;

        public double Last
        {
            get { return _last; }
            set 
            {
                _last = value;
                NotifyOfPropertyChange(() => Last);
            }
        }

        private double _ask = 0.0;

        public double Ask
        {
            get { return _ask; }
            set 
            {
                _ask = value;
                NotifyOfPropertyChange(() => Ask);
            }
        }


        private double _bid = 0.0;

        public double Bid
        {
            get { return _bid; }
            set 
            {
                _bid = value;
                NotifyOfPropertyChange(() => Bid);
            }
        }

        private string _selectedTIF = "DAY";

        public string SelectedTIF
        {
            get { return _selectedTIF; }
            set 
            {
                _selectedTIF = value;
                NotifyOfPropertyChange(() => SelectedTIF);
            }
        }


        private List<string> _tifs = new List<string> { "DAY", "GTC" };

        public List<string> TIFS
        {
            get { return _tifs; }
            set { _tifs = value; }
        }


        private string _primaryExchange = "NASDAQ";

        public string PrimaryExchange
        {
            get { return _primaryExchange; }
            set 
            {
                _primaryExchange = value;
                NotifyOfPropertyChange(() => PrimaryExchange);
            }
        }


        private int _visible = 100;

        public int Visible
        {
            get { return _visible; }
            set 
            {
                _visible = value;
                NotifyOfPropertyChange(() => Visible);
            }
        }

        private string _selectedType = "LMT";

        public string SelectedType
        {
            get { return _selectedType; }
            set 
            {
                _selectedType = value;
                NotifyOfPropertyChange(() => SelectedType);
            }
        }


        private List<string> _profitStopLimit = new List<string> { "0.25", "0.5", "0.75", "1", "1.50", "2", "2%", "5%", "10%", "20%", };

        public List<string> ProfitStopLimit
        {
            get { return _profitStopLimit; }
            set 
            {
                _profitStopLimit = value;
                NotifyOfPropertyChange(() => ProfitStopLimit);
            }
        }

        private string _selectedProfitStopLimit;

        public string SelectedProftiStopLimit
        {
            get { return _selectedProfitStopLimit; }
            set 
            {
                _selectedProfitStopLimit = value;
                NotifyOfPropertyChange(() => _selectedProfitStopLimit);
            }
        }


        private List<string> _types = new List<string> { "LMT", "MKT", "STP" };

        public List<string> Types
        {
            get { return _types; }
            set 
            {
                _types = value;
                NotifyOfPropertyChange(() => Types);
            }
        }


        private string _selectedMarket = "SMART";

        public string SelectedMarket
        {
            get { return _selectedMarket; }
            set 
            {
                _selectedMarket = value;
                NotifyOfPropertyChange(() => SelectedMarket);
            }
        }


        private List<string> _markets = new List<string> { "SMART", "NYSE" };

        public List<string> Markets
        {
            get { return _markets; }
            set { _markets = value; }
        }


        private double _lmtPrice;

        public double LMTPrice
        {
            get { return _lmtPrice; }
            set 
            { 
                _lmtPrice = value;
                NotifyOfPropertyChange(() => LMTPrice);
            }
        }


        private int _quantity = 100;

        public int Quantity
        {
            get { return _quantity; }
            set 
            {
                _quantity = value;
                NotifyOfPropertyChange(() => Quantity);
            }
        }

        private double _takeProfit = 0.00;

        public double TakeProfit
        {
            get 
            {
                if (SelectedProftiStopLimit == null) return Last + 1.00;

                switch (SelectedProftiStopLimit)
                {
                    case "1":
                        return Last + 1.00;
                    case "0.25":
                        return Last + 0.25;
                    case "0.5":
                        return Last + 0.50;
                    case "0.75":
                        return Last + 0.75;
                    case "1.50":
                        return Last + 1.50;
                    case "2":
                        return Last + 2.00;
                    case "5%":
                        return ((Quantity * LMTPrice) + (Quantity * LMTPrice * 0.05)) / Quantity;
                    case "10%":
                        return ((Quantity * LMTPrice) + (Quantity * LMTPrice * 0.10)) / Quantity;
                    case "2%":
                        return ((Quantity * LMTPrice) + (Quantity * LMTPrice * 0.02)) / Quantity;
                    case "20%":
                        return ((Quantity * LMTPrice) + (Quantity * LMTPrice * 0.20)) / Quantity;
                    default:
                        return Last - 1.00;
                }
            }
            set 
            {
                _takeProfit = value;
                NotifyOfPropertyChange(() => TakeProfit);
            }
        }

        private double _stopLoss = 0.00;

        public double StopLoss
        {
            get 
            {
                if (SelectedProftiStopLimit == null) return Last - 1.00;

                switch (SelectedProftiStopLimit)
                {
                    case "1":
                        return Last - 1.00;
                    case "0.25":
                        return Last - 0.25;
                    case "0.5":
                        return Last - 0.50;
                    case "0.75":
                        return Last - 0.75;
                    case "1.50":
                        return Last - 1.50;
                    case "2":
                        return Last - 2.00;
                    case "5%":
                        return ((Quantity * LMTPrice) - (Quantity * LMTPrice* 0.05)) / Quantity;
                    case "10%":
                        return ((Quantity * LMTPrice) - (Quantity * LMTPrice * 0.10)) / Quantity;
                    case "2%":
                        return ((Quantity * LMTPrice) - (Quantity * LMTPrice * 0.02)) / Quantity;
                    case "20%":
                        return ((Quantity * LMTPrice) - (Quantity * LMTPrice * 0.20)) / Quantity;
                    default:
                        return Last - 1.00;
                }

            }
            set 
            {
                _stopLoss = value;
                NotifyOfPropertyChange(() => StopLoss);
            }
        }



        private string _ticker = "AAPL";

        public string Ticker
        {
            get { return _ticker; }
            set 
            {
                _ticker = value;
                NotifyOfPropertyChange(() => Ticker);
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

        private int _timerCounter;

        public int TimerCounter
        {
            get { return _timerCounter; }
            set 
            {
                _timerCounter = value;
                NotifyOfPropertyChange(() => TimerCounter);
            }
        }

        public void UpdateLimit(RoutedEventArgs e)
        {
            var content = (double)(e.Source as Button).Content;
            LMTPrice = content;
        }


        private void StartTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Tickevent;
            timer.Start();
        }

        private void Tickevent(object sender, EventArgs e)
        {
            LMTPrice = Bid;
        }

        public void SymbolKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                getData();
            }
        }

        public void CancelAll()
        {
            ibClient.ClientSocket.reqGlobalCancel();
        }

        public void CancelLast()
        {
            ibClient.ClientSocket.cancelOrder(OrderId - 1);
        }

    }
}
