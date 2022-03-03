using Caliburn.Micro;
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

        public MainMenuViewModel(IEventAggregator events)
        {
           _events = events;
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
    }
}
