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

        public ShellViewModel(IEventAggregator events, LoginViewModel loginVM)
        {
            _events = events;
            _loginVM = loginVM;

            _events.Subscribe(this);

            ActivateItem(_loginVM);
        }

        public void Handle(LogOnEvent message)
        {
            throw new NotImplementedException();
        }
    }
}
