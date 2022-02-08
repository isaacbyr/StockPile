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
    public class StrategyViewModel: Screen
    {
        private readonly IStrategyEndpoint _strategyEndpoint;

        public StrategyViewModel(IStrategyEndpoint strategyEndpoint)
        {
            _strategyEndpoint = strategyEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadStrategies();
        }

        private async Task LoadStrategies()
        {
            await _strategyEndpoint.LoadStrategies();
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


    }
}
