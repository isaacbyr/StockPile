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
    [RoutePrefix("api/friendrequests")]
    public class FriendRequestsController : ApiController
    {
        [HttpGet]
        public List<FriendModel> LoadFriendRequests()
        {
            var friendRequestsData = new FriendRequestsData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return friendRequestsData.LoadFriendRequests(id);
        }

        [HttpDelete]
        [Route("{followerId}")]
        public void DeleteRequest(string followerId)
        {
            var friendRequestData = new FriendRequestsData();

            string followeeId = RequestContext.Principal.Identity.GetUserId();

            friendRequestData.DeleteRequest(followeeId, followerId);
        }

        [HttpPost]
        public void PostRequest(FriendRequestModel request)
        {
            var friendReqData = new FriendRequestsData();

            request.FollowerId = RequestContext.Principal.Identity.GetUserId();

            friendReqData.PostRequest(request);
        }
    }
}
