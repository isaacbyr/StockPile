using DataManager.Library.DataAccess.TraderPro;
using DataManager.Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers.TraderPro
{
    [RoutePrefix("api/traderealizedpl")]
    public class TradeRealizedPLController : ApiController
    {
        [HttpPost]
        public void PostRealizedPL(UpdateRealizedPLModel realizedPL)
        {
            var realizedPLData = new TradeRealizedPLData();

            //TODO: REMOVE HARDCODE OF USER ID
            //string id = RequestContext.Principal.Identity.GetUserId();
            realizedPL.UserId= "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            realizedPLData.PostRealizedPL(realizedPL);
        }

        [HttpGet]
        public List<RealizedPLChartModel> LoadHistory()
        {
            var realizedPLData = new TradeRealizedPLData();

            //TODO: REMOVE HARDCODE OF USER ID
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return realizedPLData.LoadHistory(id);
        }

        [HttpGet]
        [Route("history/{id}")]
        public List<RealizedPLChartModel> LoadHistoryByUserId(string id)
        {
            var realizedPLData = new TradeRealizedPLData();

            return realizedPLData.LoadHistory(id);
        }

        [HttpGet]
        [Route("{id}")]
        public List<LeaderboardModel> LoadById(string id)
        {
            var realizedPLData = new TradeRealizedPLData();

            return realizedPLData.LoadById(id);
        }
    }
}
