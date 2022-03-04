using Caliburn.Micro;
using SlackConnector;
using SlackConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.ViewModels.Slack
{
    public class SlackTraderViewModel: Screen
    {

        public SlackTraderViewModel()
        {

        }

        protected override async void OnViewLoaded(object view)
        {
            ISlackConnector connector = new SlackConnector.SlackConnector();
            ISlackConnection connection = await connector.Connect("xoxb-2948513536290-3175021330071-w8TsQWWk9CaOJUfppb6qbKjA");
            connection.OnMessageReceived += MessageReceived;
            connection.OnDisconnect += Disconnected;
        }

        private Task MessageReceived(SlackMessage message)
        {
            throw new NotImplementedException();
        }

        private void Disconnected()
        {
            throw new NotImplementedException();
        }

    }
}
