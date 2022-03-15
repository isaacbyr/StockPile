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
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        [Authorize]
        [HttpGet]
        public List<LoggedInUserModel> Get ()
        {
            string id = RequestContext.Principal.Identity.GetUserId();

            var userData = new UserData();

            return userData.GetUserById(id);
        }

        [HttpGet]
        [Route("{searchInput}")]
        public List<FriendModel> FriendSearch(string searchInput)
        {
            var userData = new UserData();

            return userData.FriendSearch(searchInput);
        }

        [HttpPost]
        public void Post(LoggedInUserModel user)
        {
            var userData = new UserData();

            user.Id = RequestContext.Principal.Identity.GetUserId();

            userData.PostNewUser(user);
        }
    }
}
