using DataManager.Library.DataAccess;
using DataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers
{
    [RoutePrefix("api/friends")]
    public class FriendsController : ApiController
    {
        [HttpGet]
        public List<FriendModel> LoadFriends()
        {
            var friendsData = new FriendsData();

            //TODO: Remove hardcode of user id
            string id = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            return friendsData.LoadFriends(id);
        }

        [HttpPost]
        public void PostFriendship(NewFriendshipModel friends)
        {
            var friendsData = new FriendsData();

            // TODO: Remove hardcode of user id
            friends.FolloweeId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            //Post first relationship
            friendsData.PostFriendship(friends.FolloweeId, friends.FollowerId);

            // post reverse relationship
            friendsData.PostFriendship(friends.FollowerId, friends.FolloweeId);

        }
    }
}
