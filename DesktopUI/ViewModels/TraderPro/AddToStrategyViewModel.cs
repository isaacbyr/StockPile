using Caliburn.Micro;
using DesktopUI.Library.Api.TraderPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.TraderPro
{
    public class AddToStrategyViewModel: Screen
    {
        private readonly IStrategyEndpoint _strategyEndpoint;

        public AddToStrategyViewModel(IStrategyEndpoint strategyEndpoint)
        {
           _strategyEndpoint = strategyEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadStrategies();
        }

        private async Task LoadStrategies()
        {
            var results = await _strategyEndpoint.LoadStrategies();

        }

        private BindingList<StrategyModel> _strategies;

        public BindingList<StrategyModel> Strategies
        {
            get { return _strategies; }
            set { _strategies = value; }
        }


    }
}
