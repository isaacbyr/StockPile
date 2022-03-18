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
    [RoutePrefix("api/realizedpl")]
    public class RealizedPLController : ApiController
    {
        [HttpPost]
        public void PostRealizedPL(UpdateRealizedPLModel realizedPL)
        {
            var realizedPLData = new RealizedPLData();

            realizedPL.UserId = RequestContext.Principal.Identity.GetUserId();

            realizedPLData.PostRealizedPL(realizedPL);
        }

        [HttpGet]
        public List<RealizedPLChartModel> LoadHistory()
        {
            var realizedPLData = new RealizedPLData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return realizedPLData.LoadHistory(id);
        }

        [HttpGet]
        [Route("history/{id}")]
        public List<RealizedPLChartModel> LoadHistoryByUserId(string id)
        {
            var realizedPLData = new RealizedPLData();

            return realizedPLData.LoadHistory(id);
        }

        [HttpGet]
        [Route("{id}")]
        public List<LeaderboardModel> LoadById(string id)
        {
            var realizedPLData = new RealizedPLData();

            return realizedPLData.LoadById(id);
        }
    }
}
