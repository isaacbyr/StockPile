using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class FriendDisplayModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }

        public FriendDisplayModel()
        {

        }

        public FriendDisplayModel(string id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }
}
