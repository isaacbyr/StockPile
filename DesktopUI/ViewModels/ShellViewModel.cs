using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DesktopUI.Library.EventModels;

namespace DesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEvent>, IHandle<OpenPortfolioStockView>, IHandle<ReturnHomeEvent>,
        IHandle<OpenPortfolioSummaryView>
    {
        private readonly IEventAggregator _events;
        private LoginViewModel _loginVM;
        private readonly DashboardViewModel _dashboardVM;
        private readonly PortfolioStockViewModel _portfolioStockVM;
        private readonly PortfolioSummaryViewModel _portfolioSummaryVM;

        public ShellViewModel(IEventAggregator events, LoginViewModel loginVM, DashboardViewModel dashboardVM,
            PortfolioStockViewModel portfolioStockVM, PortfolioSummaryViewModel portfolioSummaryVM)
        {
            _events = events;
            _loginVM = loginVM;
            _dashboardVM = dashboardVM;
            _portfolioStockVM = portfolioStockVM;
            _portfolioSummaryVM = portfolioSummaryVM;
            _events.Subscribe(this);

            //ActivateItem(_loginVM);
            //ActivateItem(_portfolioStockVM);
            //ActivateItem(_dashboardVM);
            ActivateItem(_portfolioSummaryVM);
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_dashboardVM);
        }

        public void Handle(OpenPortfolioStockView message)
        {
            _portfolioStockVM.TickerOnLoad = message.Ticker;
            ActivateItem(_portfolioStockVM);
        }

        public void Handle(ReturnHomeEvent message)
        {
            ActivateItem(_dashboardVM);
        }

        public void Handle(OpenPortfolioSummaryView message)
        {
            ActivateItem(_portfolioSummaryVM);
        }
    }
}
