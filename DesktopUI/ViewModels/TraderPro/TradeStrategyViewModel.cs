using Caliburn.Micro;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.TraderPro
{
    public class TradeStrategyViewModel: Screen
    {
        private readonly ITWSTradingEndpoint _tWSTradingEndpoint;


        public TradeStrategyViewModel(string ticker, int buyShares, int sellShares, string ma1, string ma2, 
            string indicator, string interval, string range, ITWSTradingEndpoint tWSTradingEndpoint)
        {
            Ticker = ticker;
            BuyShares = buyShares;
            SellShares = sellShares;
            MA1 = ma1;
            MA2 = ma2;
            Indicator = indicator;
            Interval = interval;
            Range = range;
            _tWSTradingEndpoint = tWSTradingEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await PostTWSTradeStrategy();
            await LoadTWSStrategies();
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

    }
}
