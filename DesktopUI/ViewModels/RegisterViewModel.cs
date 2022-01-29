using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.EventModels;
using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels
{
    public class RegisterViewModel: Screen
    {
        private readonly IApiHelper _apiHelper;
        private readonly IUserEndpoint _userEndpoint;
        private readonly IEventAggregator _events;

        public RegisterViewModel(IApiHelper apiHelper, IUserEndpoint userEndpoint, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _userEndpoint = userEndpoint;
            _events = events;
        }


        private string _firstName = "Sue";

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }

        private string _lastName = "Beckley";

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
        }

        private string _userName = "sbeckley@shaw.ca";

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        private string _password = "Lucy#12";

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        private string _confirmPassword = "Lucy#12";

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                NotifyOfPropertyChange(() => ConfirmPassword);
            }
        }

        public async Task Register()
        {
            var user = new RegisterUserModel();
            var logUser = new LoggedInUserModel();


            user.Email = UserName;
            user.Password = Password;
            user.ConfirmPassword = ConfirmPassword;

            logUser.FirstName = FirstName;
            logUser.LastName = LastName;
            logUser.Email = UserName;

            try
            {
                await _apiHelper.RegisterUser(user);

                var result = await _apiHelper.Authenticate(user.Email, user.Password);

                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                await _userEndpoint.LogUser(logUser);

                _events.PublishOnUIThread(new LogOnEvent());
            }
            catch(Exception ex)
            {
                // TODO: Add dialog window
                throw new Exception(ex.Message);
            }
        }

        public void Login()
        {
            _events.PublishOnUIThread(new OpenLoginView());
        }
    }
}
