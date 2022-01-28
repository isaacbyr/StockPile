using DesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public interface IFriendRequestEndpoint
    {
        Task<List<FriendModel>> LoadFriendRequests();
        Task DeleteRequest(string followerId);
        Task PostRequest(FriendRequestModel request);
    }
}