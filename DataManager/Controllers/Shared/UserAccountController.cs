﻿using DataManager.Library.DataAccess;
using DataManager.Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace DataManager.Controllers
{
    [RoutePrefix("api/useraccount")]
    //[EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class UserAccountController : ApiController
    {
        [HttpGet]
        public UserPortfolioOverviewModel LoadPortfolioOverview()
        {
            var userAccountData = new UserAccountData();

            //string id = RequestContext.Principal.Identity.GetUserId();

            //TODO: REMOVE HARDCODE OF USER ID
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";


            return userAccountData.LoadPortfolioOverview(id);
        }

        [HttpGet]
        [Route("trades")]
        public UserPortfolioOverviewModel LoadTradesPortfolioOverview()
        {
            var userAccountData = new UserAccountData();

            //TODO: REMOVE HARDCODE OF USER ID
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return userAccountData.LoadTradesPortfolioOverview(id);
        }


        [HttpPut]
        [Route("updatebalance/portfolio")]
        public void UpdatePortfolioAccountBalance(UpdateUserAccountModel update)
        {
            var userAccountData = new UserAccountData();

            //TODO: REMOVE HARDCODE OF USER ID
            update.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";
            //update.UserId = RequestContext.Principal.Identity.GetUserId();

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
        [Route("tradesbalance")]
        public decimal LoadAccountBalance()
        {
            var userAccountData = new UserAccountData();

            //TODO: 
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return userAccountData.LoadTradesAccountBalance(id);
        }


        [HttpPut]
        [Route("sale")]
        public decimal UpdateAfterSale(UpdateUserAccountModel update)
        {
            var userAccountData = new UserAccountData();

            //TODO: 
            //string id = RequestContext.Principal.Identity.GetUserId();
            update.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return userAccountData.UpdateAfterSale(update);
        }


        [HttpPut]
        [Route("sale/trades")]
        public decimal UpdateTradesAfterSale(UpdateUserAccountModel update)
        {
            var userAccountData = new UserAccountData();

            //TODO: CHANGE HARDCODE
            update.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";
            //update.UserId = RequestContext.Principal.Identity.GetUserId();

            return userAccountData.UpdateTradesAfterSale(update);
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
