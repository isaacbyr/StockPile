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

        [HttpPut]
        [Route("updatebalance")]
        public void UpdateAccountBalance(UpdateUserAccountModel update)
        {
            var userAccountData = new UserAccountData();

            update.UserId = RequestContext.Principal.Identity.GetUserId();

            userAccountData.UpdateAccountBalance(update);
        }

        [HttpPut]
        [Route("sale")]
        public decimal UpdateAfterSale(UpdateUserAccountModel update)
        {
            var userAccountData = new UserAccountData();

            update.UserId = RequestContext.Principal.Identity.GetUserId();

            return userAccountData.UpdateAfterSale(update);
        }

        [HttpPost]
        public void PostNewUserAccount(UserAccountModel userAccount)
        {
            var userAccountData = new UserAccountData();

            userAccount.UserId = RequestContext.Principal.Identity.GetUserId();

            userAccountData.PostNewUserAccount(userAccount);
        }
    }
}
