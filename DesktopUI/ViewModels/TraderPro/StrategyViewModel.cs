using Caliburn.Micro;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.EventModels.TraderPro;
using DesktopUI.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.TraderPro
{
    public class StrategyViewModel: Screen
    {
        private readonly IStrategyEndpoint _strategyEndpoint;
        private readonly IEventAggregator _events;

        public StrategyViewModel(IStrategyEndpoint strategyEndpoint, IEventAggregator events)
        {
            _strategyEndpoint = strategyEndpoint;
            _events = events;
        }

        protected override async void OnViewLoaded(object view)
        {
           await LoadStrategies();
        }

        private async Task LoadStrategies()
        {
            var results = await _strategyEndpoint.LoadStrategies();
            Strategies = new BindingList<StrategyModel>(results);
        }

        private BindingList<StrategyModel> _strategiess;

        public BindingList<StrategyModel> Strategies
        {
            get { return _strategiess; }
            set 
            {
                _strategiess = value;
                NotifyOfPropertyChange(() => Strategies);
            }
        }

        private StrategyModel _selectedStrategy;

        public StrategyModel SelectedStrategy
        {
            get { return _selectedStrategy; }
            set 
            {
                _selectedStrategy = value;
                NotifyOfPropertyChange(() => SelectedStrategy);
            }
        }

        private BindingList<StrategyItemModel>_stocks;

        public BindingList<StrategyItemModel> Stocks
        {
            get { return _stocks; }
            set 
            {
                _stocks = value;
                NotifyOfPropertyChange(() => Stocks);
            }
        }

        private StrategyItemModel _selectedStock;

        public StrategyItemModel SelectedStock
        {
            get { return _selectedStock; }
            set 
            { 
                _selectedStock = value;
                NotifyOfPropertyChange(() => SelectedStock);
            }
        }

        private string _selectedName;

        public string SelectedName
        {
            get { return _selectedName; }
            set 
            {
                _selectedName = value;
                NotifyOfPropertyChange(() => SelectedName);
            }
        }

        private string _selectedInterval;

        public string SelectedInterval
        {
            get { return _selectedInterval; }
            set 
            {
                _selectedInterval = value;
                NotifyOfPropertyChange(() => SelectedInterval);
            }
        }

        private string _selectedRange;

        public string SelectedRange
        {
            get { return _selectedRange; }
            set 
            {
                _selectedRange = value;
                NotifyOfPropertyChange(() => SelectedRange);
            }
        }

        private string _selectedIndicator;

        public string SelectedIndicator
        {
            get { return _selectedIndicator; }
            set 
            {
                _selectedIndicator = value;
                NotifyOfPropertyChange(() => SelectedIndicator);
            }
        }

        private string _selectedMA1;

        public string SelectedMA1
        {
            get { return _selectedMA1; }
            set 
            { 
                _selectedMA1 = value;
                NotifyOfPropertyChange(() => SelectedMA1);
            }
        }

        private string _selectedMA2;

        public string SelectedMA2
        {
            get { return _selectedMA2; }
            set 
            {
                _selectedMA2 = value;
                NotifyOfPropertyChange(() => SelectedMA2);
            }
        }

        private double _profitLoss;

        public double ProfitLoss
        {
            get { return _profitLoss; }
            set 
            {
                _profitLoss = value;
                NotifyOfPropertyChange(() => ProfitLoss);
            }
        }



        public async Task StrategyView()
        {
            if(SelectedStrategy == null)
            {
                return;
            }

            var stocks = await _strategyEndpoint.GetStrategyStocks(SelectedStrategy.Id);
            Stocks = new BindingList<StrategyItemModel>(stocks);

            ProfitLoss = (double)Stocks.Sum(x => x.ProfitLoss);
            SelectedName = SelectedStrategy.Name;
            SelectedRange = SelectedStrategy.Range;
            SelectedInterval = SelectedStrategy.Interval;
            SelectedIndicator = SelectedStrategy.Indicator;
            SelectedMA2 = SelectedStrategy.MA2;
            SelectedMA1 = SelectedStrategy.MA1;
        }

        public void TradeWithStrategy()
        {
            if (SelectedStock == null || SelectedStrategy == null) return;
            _events.PublishOnUIThread(
                new OpenTradeStrategyView(
                    SelectedStock.Ticker, SelectedStock.BuyShares, SelectedStock.SellShares,
                    SelectedMA1, SelectedMA2, SelectedIndicator, SelectedInterval, SelectedRange));
        }

        public void TradeCrossovers()
        {
            _events.PublishOnUIThread(new LaunchTraderProEvent());
        }

        public void PaperTradeLive()
        {
            _events.PublishOnUIThread(new OpenPaperTradeView());
        }

        public void PaperTrade()
        {
            _events.PublishOnUIThread(new OpenPaperTradeView());
        }

        public void Menu()
        {
            _events.PublishOnUIThread(new OpenMainMenuEvent());
        }
    }
}
