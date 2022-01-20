using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DesktopUI.Library.EventModels;

namespace DesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEvent>
    {
        private readonly IEventAggregator _events;
        private LoginViewModel _loginVM;
        private readonly DashboardViewModel _dashboardVM;

        public ShellViewModel(IEventAggregator events, LoginViewModel loginVM, DashboardViewModel dashboardVM)
        {
            _events = events;
            _loginVM = loginVM;
            _dashboardVM = dashboardVM;
            
            
            _events.Subscribe(this);

            //ActivateItem(_loginVM);
            ActivateItem(_dashboardVM);
        }

        public void Handle(LogOnEvent message)
        {
            throw new NotImplementedException();
        }
    }
}
