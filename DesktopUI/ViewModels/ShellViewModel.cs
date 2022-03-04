using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DesktopUI.Library.EventModels;
using DesktopUI.ViewModels.Slack;
using DesktopUI.ViewModels.TraderPro;

namespace DesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEvent>, IHandle<OpenPortfolioStockView>, IHandle<ReturnHomeEvent>,
        IHandle<OpenPortfolioSummaryView>, IHandle<OpenSocialView>, IHandle<OpenRegisterView>, IHandle<OpenLoginView>, IHandle<LogOffEvent>,
        IHandle<ExitAppEvent>, IHandle<LaunchPortoflioProEvent>, IHandle<LaunchTraderProEvent>, IHandle<LaunchTWSTradingEvent>
    {
        private readonly IEventAggregator _events;
        private LoginViewModel _loginVM;
        private readonly DashboardViewModel _dashboardVM;
        private readonly PortfolioStockViewModel _portfolioStockVM;
        private readonly PortfolioSummaryViewModel _portfolioSummaryVM;
        private readonly SocialViewModel _socialVM;
        private readonly RegisterViewModel _registerVM;
        private readonly TraderMainViewModel _traderMainVM;
        private readonly IWindowManager _window;
        private readonly PaperTradeViewModel _paperTradeVM;
        private readonly LiveTradesViewModel _liveTradesVM;
        private readonly IBViewModel _ibVM;
        private readonly MainMenuViewModel _mainMenuVM;
        private readonly SlackTraderViewModel _slackTraderVM;

        public ShellViewModel(IEventAggregator events, LoginViewModel loginVM, DashboardViewModel dashboardVM,
            PortfolioStockViewModel portfolioStockVM, PortfolioSummaryViewModel portfolioSummaryVM,
            SocialViewModel socialVM, RegisterViewModel registerVM, TraderMainViewModel traderMainVM, IWindowManager window,
            PaperTradeViewModel paperTradeVM, LiveTradesViewModel liveTradesVM, IBViewModel ibVM, MainMenuViewModel mainMenuVM,
            SlackTraderViewModel slackTraderVM)
        {
            _events = events;
            _loginVM = loginVM;
            _dashboardVM = dashboardVM;
            _portfolioStockVM = portfolioStockVM;
            _portfolioSummaryVM = portfolioSummaryVM;
            _socialVM = socialVM;
            _registerVM = registerVM;
            _traderMainVM = traderMainVM;
            _window = window;
            _paperTradeVM = paperTradeVM;
            _liveTradesVM = liveTradesVM;
            _ibVM = ibVM;
            _mainMenuVM = mainMenuVM;
            _slackTraderVM = slackTraderVM;
            _events.Subscribe(this);

            //ActivateItem(socialVM);
            //ActivateItem(_loginVM);
            // ActivateItem(_mainVM);
            //ActivateItem(_registerVM);
            //ActivateItem(_portfolioStockVM);
            //ActivateItem(_dashboardVM);
            //ActivateItem(_portfolioSummaryVM);
            //ActivateItem(_paperTradeVM);
            //ActivateItem(_liveTradesVM); 
            //ActivateItem(_ibVM);
            ActivateItem(_mainMenuVM);
            //ActivateItem(_slackTraderVM);
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

        public void Handle(LaunchPortoflioProEvent message)
        {
            ActivateItem(_dashboardVM);
        }

        public void Handle(LaunchTraderProEvent message)
        {
            ActivateItem(_traderMainVM);
        }

        public void Handle(LaunchTWSTradingEvent message)
        {
            ActivateItem(_ibVM);
        }
    }
}
