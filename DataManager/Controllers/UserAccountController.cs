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
    [RoutePrefix("api/useraccount")]
    public class UserAccountController : ApiController
    {
        [HttpGet]
        public UserPortfolioOverviewModel LoadPortfolioOverview()
        {
            var userAccountData = new UserAccountData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return userAccountData.LoadPortfolioOverview(id);
        }
    }
}
