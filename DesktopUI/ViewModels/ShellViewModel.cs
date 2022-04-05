using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.EventModels.PortfolioPro;
using DesktopUI.Library.EventModels.TraderPro;
using DesktopUI.ViewModels.PortfolioPro;
using DesktopUI.ViewModels.TraderPro;

namespace DesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEvent>, IHandle<OpenPortfolioStockView>, IHandle<ReturnHomeEvent>,
        IHandle<OpenPortfolioSummaryView>, IHandle<OpenSocialView>, IHandle<OpenRegisterView>, IHandle<OpenLoginView>, IHandle<LogOffEvent>,
        IHandle<ExitAppEvent>, IHandle<LaunchPortoflioProEvent>, IHandle<LaunchTraderProEvent>, IHandle<LaunchTWSTradingEvent>,
        IHandle<OpenStrategiesView>, IHandle<OpenPaperTradeLiveView>, IHandle<OpenMainMenuEvent>, IHandle<OpenPaperTradeView>,
        IHandle<OpenTradeStrategyView>, IHandle<OpenTraderPerformanceView>, IHandle<OpenTraderDashboardView>
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
        private readonly StrategyViewModel _strategyVM;
        private readonly TradeStrategyViewModel _tradeStrategyVM;
        private readonly TraderProDashboardViewModel _tradeProDashboardVM;
        private readonly TraderPortfolioOverviewViewModel _traderPortoflioOverviewVM;
        private readonly FriendProfileViewModel _friendProfileVM;

        public ShellViewModel(IEventAggregator events, LoginViewModel loginVM, DashboardViewModel dashboardVM,
            PortfolioStockViewModel portfolioStockVM, PortfolioSummaryViewModel portfolioSummaryVM,
            SocialViewModel socialVM, RegisterViewModel registerVM, TraderMainViewModel traderMainVM, IWindowManager window,
            PaperTradeViewModel paperTradeVM, LiveTradesViewModel liveTradesVM, IBViewModel ibVM, MainMenuViewModel mainMenuVM,
             StrategyViewModel strategyVM, TradeStrategyViewModel tradeStrategyVM,
            TraderProDashboardViewModel tradeProDashboardVM, 
            TraderPortfolioOverviewViewModel traderPortoflioOverviewVM, FriendProfileViewModel friendProfileVM)
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
            _strategyVM = strategyVM;
            _tradeStrategyVM = tradeStrategyVM;
            _tradeProDashboardVM = tradeProDashboardVM;
            _traderPortoflioOverviewVM = traderPortoflioOverviewVM;
            _friendProfileVM = friendProfileVM;
            _events.Subscribe(this);

            //ActivateItem(socialVM);
            ActivateItem(_loginVM);
            // ActivateItem(_mainVM);
            //ActivateItem(_registerVM);
            //ActivateItem(_portfolioStockVM);
            //ActivateItem(_dashboardVM);
            //ActivateItem(_portfolioSummaryVM);
            //ActivateItem(_paperTradeVM);
            // ActivateItem(_liveTradesVM);
            //ActivateItem(_ibVM);
            // ActivateItem(_mainMenuVM);
            //ActivateItem(_tradeProDashboardVM);
            //ActivateItem(_twitterScreenerVM);
            //ActivateItem(_traderPortoflioOverviewVM);
            //ActivateItem(_tradeStrategyVM);
            //ActivateItem(_traderMainVM);
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_mainMenuVM);
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

        public void Handle(OpenStrategiesView message)
        {
            ActivateItem(_strategyVM);
        }

        public void Handle(OpenPaperTradeLiveView message)
        {
            ActivateItem(_liveTradesVM);
        }

        public void Handle(OpenMainMenuEvent message)
        {
            ActivateItem(_mainMenuVM);
        }

        public void Handle(OpenPaperTradeView message)
        {
            ActivateItem(_paperTradeVM);
        }

        public void Handle(OpenTradeStrategyView message)
        {

            if(message.AddNew == true)
            {
                _tradeStrategyVM.Ticker = message.Ticker;
                _tradeStrategyVM.BuyShares = message.BuyShares;
                _tradeStrategyVM.SellShares = message.SellShares;
                _tradeStrategyVM.Indicator = message.Indicator;
                _tradeStrategyVM.Interval = message.Interval;
                _tradeStrategyVM.MA1 = message.MA1;
                _tradeStrategyVM.MA2 = message.MA2;
                _tradeStrategyVM.Range = message.Range;
                _tradeStrategyVM.AddNew = true;
            }
            else
            {
                _tradeStrategyVM.AddNew = false;
            }



            ActivateItem(_tradeStrategyVM);
        }

        public void Handle(OpenTraderPerformanceView message)
        {
            ActivateItem(_traderPortoflioOverviewVM);
        }

        public void Handle(OpenTraderDashboardView message)
        {
            ActivateItem(_tradeProDashboardVM);
        }

       
    }
}
