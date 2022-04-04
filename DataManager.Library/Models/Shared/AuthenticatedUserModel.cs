using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager.Library.Models.Shared
{
    public class AuthenticatedUserModel
    {
        public string Access_Token { get; set; }
        public string Email { get; set; }
    }
}
