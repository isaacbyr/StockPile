﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using IBApi;

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

            while(ibClient.NextOrderId <= 0) { }

            ibClient.ibVM = this;

            // load in order id 
            OrderId = ibClient.NextOrderId;

            getData();
        }

        private void getData()
        {
            ibClient.ClientSocket.cancelMktData(1); // cancel market data

            // create a new contract to specuft the security we are searching for
            var contract = new Contract();

            // Create a new TagValueList Object 
            List<TagValue> mktDataOptions = new List<TagValue>();

            // Set the stock ticker to get data for
            contract.Symbol = "AAPL";
            // Set the security type to stk for a stock
            contract.SecType = "STK";
            // Use "SMART" as the general exchange;
            contract.Exchange = "SMART";
            // Set Exchange
            contract.PrimaryExch = "NYSE";
            // Set currency
            contract.Currency = "USD";

            // request to use delayed data
            ibClient.ClientSocket.reqMarketDataType(3);

            ibClient.ClientSocket.reqMktData(1, contract, "", false, false, mktDataOptions);

        }

        public void GetTickerPrice(string _tickerPrice)
        {
            string[] tickerPrice = new string[] { _tickerPrice };
            tickerPrice = _tickerPrice.Split(',');

            if (Convert.ToInt32(tickerPrice[0]) == 1)
            {
                if (Convert.ToInt32(tickerPrice[1]) == 68)
                {
                    // LAST QUOTE FOR DELAYED DATA
                    Console.WriteLine("DELAYED QUOTE: ", tickerPrice[2]);
                }
                else if (Convert.ToInt32(tickerPrice[1]) == 67)
                {
                    // DELAYED ASK QUOTE
                    Console.WriteLine("DELAYED ASK QUOTE: ", tickerPrice[2]);
                }
                else if (Convert.ToInt32(tickerPrice[1]) == 66)
                {
                    // DELAYED BIG QUOTE
                    Console.WriteLine("DELAYED BID QUOTE", tickerPrice[2]);
                }
            }


        }

        public void Connect()
        {

        }

        public void Disconnect()
        {
            ibClient.ClientSocket.eDisconnect();
        }

        public void Buy()
        {
            string side = "BUY";
            send_order(side);
        }

        public void Sell()
        {
            string side = "SELL";
            send_order(side);
        }

        public void send_order(string side)
        {
            // create a new contract to specify the security we are looking for
            var contract = new Contract();

            contract.Symbol = "MSFT";
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.PrimaryExch = "ISLAND";
            contract.Currency = "UDS";

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
            order.OutsideRth = false;

            //place the order
            ibClient.ClientSocket.placeOrder(OrderId, contract, order);

            OrderId++;


        }

        private double _ask;

        private double _last;

        public double Last
        {
            get { return _last; }
            set 
            {
                _last = value;
                NotifyOfPropertyChange(() => Last);
            }
        }


        public double Ask
        {
            get { return _ask; }
            set 
            {
                _ask = value;
                NotifyOfPropertyChange(() => Ask);
            }
        }


        private double _bid;

        public double Bid
        {
            get { return _bid; }
            set 
            {
                _bid = value;
                NotifyOfPropertyChange(() => Bid);
            }
        }

        private string _selectedTIF;

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

        private string _selectedType;

        public string SelectedType
        {
            get { return _selectedType; }
            set 
            {
                _selectedType = value;
                NotifyOfPropertyChange(() => SelectedType);
            }
        }



        private List<string> _types = new List<string> { "LMT", "MKT" };

        public List<string> Types
        {
            get { return _types; }
            set 
            {
                _types = value;
                NotifyOfPropertyChange(() => Types);
            }
        }


        private string _selectedMarket;

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

        //TODO: ADD IN TIMER FUNCTION TO CHANGE LIMIT PRICE
    }
}