using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IFriendsEndpoint
    {
        Task<List<FriendModel>> LoadFriends();
        Task PostFriendship(string followerId);
    }
}