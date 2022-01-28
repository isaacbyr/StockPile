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
    [RoutePrefix("api/friendrequests")]
    public class FriendRequestsController : ApiController
    {
        [HttpGet]
        public List<FriendModel> LoadFriendRequests()
        {
            var friendRequestsData = new FriendRequestsData();

            //TODO: Remove hardcode of user id
            string id = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            return friendRequestsData.LoadFriendRequests(id);
        }

        [HttpDelete]
        [Route("{followerId}")]
        public void DeleteRequest(string followerId)
        {
            var friendRequestData = new FriendRequestsData();

            //TODO: Remove hardcode of user id
            string followeeId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            friendRequestData.DeleteRequest(followeeId, followerId);
        }

        [HttpPost]
        public void PostRequest(FriendRequestModel request)
        {
            var friendReqData = new FriendRequestsData();

            //TODO: Remove hardcode of user id
            request.FollowerId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            friendReqData.PostRequest(request);
        }
    }
}
