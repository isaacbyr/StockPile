using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IUserEndpoint
    {
        Task<List<FriendModel>> FriendSearch(string searchInput);
        Task LogUser(LoggedInUserModel user);
    }
}