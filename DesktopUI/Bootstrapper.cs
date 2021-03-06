using Caliburn.Micro;
using DesktopUI.Library.Api;
using DesktopUI.Library.Api.TraderPro;
using DesktopUI.Library.Models;
using DesktopUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopUI
{
    public class Bootstrapper: BootstrapperBase
    {

        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IApiHelper, ApiHelper>()
                .Singleton<IStrategyEndpoint, StrategyEndpoint>()
                .Singleton<IFriendRequestEndpoint, FriendRequestEndpoint>()
                .Singleton<IFriendsEndpoint, FriendsEndpoint>()
                .Singleton<IUserAccountEndpoint, UserAccountEndpoint>()
                .Singleton<IUserEndpoint, UserEndpoint>()
                .Singleton<IPortfolioEndpoint, PortfolioEndpoint>()
                .Singleton<IWatchListEndpoint, WatchListEndpoint>()
                .Singleton<IStockDataEndpoint, StockDataEndpoint>()
                .Singleton<IRealizedProfitLossEndpoint, RealizedProfitLossEndpoint>()
                .Singleton<ITransactionEndoint, TransactionEndpoint>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>()
                .Singleton<INewsEndpoint, NewsEndpoint>()
                .Singleton<IEventAggregator, EventAggregator>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
