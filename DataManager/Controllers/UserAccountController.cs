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
        [Route("updatebalance/portfolio")]
        public void UpdatePortfolioAccountBalance(UpdateUserAccountModel update)
        {
            var userAccountData = new UserAccountData();

            update.UserId = RequestContext.Principal.Identity.GetUserId();

            userAccountData.UpdatePortfolioAccountBalance(update);
        }

        [HttpPut]
        [Route("updatebalance/trades")]
        public void UpdateTradesAccountBalance(UpdateUserAccountModel update)
        {
            var userAccountData = new UserAccountData();

            //update.UserId = RequestContext.Principal.Identity.GetUserId();
            update.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";


            userAccountData.UpdateTradesAccountBalance(update);
        }

        [HttpGet]
        [Route("balance")]
        public decimal LoadAccountBalance()
        {
            var userAccountData = new UserAccountData();

            //TODO: 
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return userAccountData.LoadAccountBalance(id);
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
