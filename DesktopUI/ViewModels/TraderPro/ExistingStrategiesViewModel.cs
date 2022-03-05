using Caliburn.Micro;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.Models.TraderPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.TraderPro
{
    public class ExistingStrategiesViewModel: Screen
    {
        public string Ticker { get; set; }
        public int BuyShares { get; set; }
        public int SellShares { get; set; }
        public decimal ProfitLoss { get; set; }

        private readonly IStrategyEndpoint _strategyEndpoint;
        private readonly IEventAggregator _events;

        public ExistingStrategiesViewModel(IStrategyEndpoint strategyEndpoint, IEventAggregator events)
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

        private BindingList<StrategyModel> _strategies;

        public BindingList<StrategyModel> Strategies
        {
            get { return _strategies; }
            set
            {
                _strategies = value;
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

        public async Task Select()
        {
            if(SelectedStrategy != null)
            {

                var strategyStock = new StrategyItemModel
                {
                    Id = SelectedStrategy.Id,
                    Ticker = Ticker,
                    BuyShares = BuyShares,
                    SellShares = SellShares,
                    ProfitLoss = ProfitLoss
                };

                await _strategyEndpoint.PostStrategyStock(strategyStock);

                TryClose();
            }
            else
            {
                return;
            }

        }

        public void Close()
        {
            _events.PublishOnUIThread(new LaunchTraderProEvent());
        }

    }
}
