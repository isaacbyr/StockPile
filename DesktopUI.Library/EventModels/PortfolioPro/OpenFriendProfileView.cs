using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.EventModels.PortfolioPro
{
    public class OpenFriendProfileView
    {
        public string FriendId { get; set; }

        public OpenFriendProfileView(string friendId)
        {
            FriendId = friendId;
        }
    }
}
