using DataManager.Library.DataAccess;
using DataManager.Library.Models;
using Microsoft.AspNet.Identity;
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

            //TODO: REMOVE HARDCODE OF USER ID
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";
            return friendsData.LoadFriends(id);
        }

        [HttpPost]
        public void PostFriendship(NewFriendshipModel friends)
        {
            var friendsData = new FriendsData();

            friends.FolloweeId = RequestContext.Principal.Identity.GetUserId();

            //Post first relationship
            friendsData.PostFriendship(friends.FolloweeId, friends.FollowerId);

            // post reverse relationship
            friendsData.PostFriendship(friends.FollowerId, friends.FolloweeId);

        }
    }
}
