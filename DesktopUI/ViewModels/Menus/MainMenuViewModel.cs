using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels
{
    public class MainMenuViewModel: Screen
    {
        private readonly IEventAggregator _events;
        private readonly IApiHelper _apiHelper;

        public MainMenuViewModel(IEventAggregator events, IApiHelper apiHelper)
        {
            _events = events;
            _apiHelper = apiHelper;
        }

        public void LaunchPortoflioPro()
        {
            _events.PublishOnUIThread(new LaunchPortoflioProEvent());
        }

        public void LaunchTraderPro()
        {
            _events.PublishOnUIThread(new LaunchTraderProEvent());
        }

        public void LaunchTWSTrading()
        {
            _events.PublishOnUIThread(new LaunchTWSTradingEvent());
        }

        public void Logout()
        {
            _apiHelper.Logout();
            _events.PublishOnUIThread(new LogOffEvent());
        }

        public void Exit()
        {
            _events.PublishOnUIThread(new ExitAppEvent());
        }
    }
}
