using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DesktopUI.Library.EventModels;
using DesktopUI.ViewModels.TraderPro;

namespace DesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEvent>, IHandle<OpenPortfolioStockView>, IHandle<ReturnHomeEvent>,
        IHandle<OpenPortfolioSummaryView>, IHandle<OpenSocialView>, IHandle<OpenRegisterView>, IHandle<OpenLoginView>, IHandle<LogOffEvent>,
        IHandle<ExitAppEvent>
    {
        private readonly IEventAggregator _events;
        private LoginViewModel _loginVM;
        private readonly DashboardViewModel _dashboardVM;
        private readonly PortfolioStockViewModel _portfolioStockVM;
        private readonly PortfolioSummaryViewModel _portfolioSummaryVM;
        private readonly SocialViewModel _socialVM;
        private readonly RegisterViewModel _registerVM;
        private readonly TraderMainViewModel _mainVM;
        private readonly PaperTradeViewModel _paperTradeVM;

        public ShellViewModel(IEventAggregator events, LoginViewModel loginVM, DashboardViewModel dashboardVM,
            PortfolioStockViewModel portfolioStockVM, PortfolioSummaryViewModel portfolioSummaryVM,
            SocialViewModel socialVM, RegisterViewModel registerVM, TraderMainViewModel mainVM, PaperTradeViewModel paperTradeVM)
        {
            _events = events;
            _loginVM = loginVM;
            _dashboardVM = dashboardVM;
            _portfolioStockVM = portfolioStockVM;
            _portfolioSummaryVM = portfolioSummaryVM;
            _socialVM = socialVM;
            _registerVM = registerVM;
            _mainVM = mainVM;
            _paperTradeVM = paperTradeVM;


            _events.Subscribe(this);

            //ActivateItem(socialVM);
            //ActivateItem(_loginVM);
           // ActivateItem(_mainVM);
            //ActivateItem(_registerVM);
            //ActivateItem(_portfolioStockVM);
            //ActivateItem(_dashboardVM);
            //ActivateItem(_portfolioSummaryVM);
            ActivateItem(_paperTradeVM);
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

        public void Handle(OpenSocialView message)
        {
            ActivateItem(_socialVM);
        }

        public void Handle(OpenRegisterView message)
        {
            ActivateItem(_registerVM);
        }

        public void Handle(OpenLoginView message)
        {
            ActivateItem(_loginVM);
        }

        public void Handle(LogOffEvent message)
        {
            ActivateItem(_loginVM);
        }

        public void Handle(ExitAppEvent message)
        {
            this.TryClose();
        }
    }
}
