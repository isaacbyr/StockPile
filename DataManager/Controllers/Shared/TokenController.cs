using DataManager.Library.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers.Shared
{
    [RoutePrefix("token")]
    public class TokenController : ApiController
    {
        [HttpPost]
        public List<AuthenticatedUserModel> NewUserLogin(FormUrlEncodedContent user)
        {

            return new List<AuthenticatedUserModel>();
        }
    }
}
