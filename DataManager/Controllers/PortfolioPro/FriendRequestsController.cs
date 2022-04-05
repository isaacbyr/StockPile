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

            //TODO: 
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return friendRequestsData.LoadFriendRequests(id);
        }

        [HttpDelete]
        [Route("{followerId}")]
        public void DeleteRequest(string followerId)
        {
            var friendRequestData = new FriendRequestsData();

            //TODO: 
            //string id = RequestContext.Principal.Identity.GetUserId();
            string followeeId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            friendRequestData.DeleteRequest(followeeId, followerId);
        }

        [HttpPost]
        public void PostRequest(FriendRequestModel request)
        {
            var friendReqData = new FriendRequestsData();

            //request.FollowerId = RequestContext.Principal.Identity.GetUserId();
            request.FollowerId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            friendReqData.PostRequest(request);
        }
    }
}
