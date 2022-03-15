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
    public class LoginViewModel: Screen
    {
        private readonly IApiHelper _apiHelper;
        private readonly IEventAggregator _events;

        public LoginViewModel(IApiHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public async Task Login()
        {
            try
            {
                var result = await _apiHelper.Authenticate(Email, Password);

                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                _events.PublishOnUIThread(new LogOnEvent());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Register()
        {
            _events.PublishOnUIThread(new OpenRegisterView());
        }
    }
}
