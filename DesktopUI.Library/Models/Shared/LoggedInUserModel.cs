using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Models
{
    public class LoggedInUserModel: ILoggedInUserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public void ResetUser()
        {
            Id = "";
            FirstName = "";
            LastName = "";
            Email = "";
        }
    }
}
