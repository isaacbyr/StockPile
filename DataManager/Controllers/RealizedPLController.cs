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

            //TODO: Change hardcode on user id
            realizedPL.UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            realizedPLData.PostRealizedPL(realizedPL);
        }

        [HttpGet]
        public List<RealizedPLChartModel> LoadHistory()
        {
            var realizedPLData = new RealizedPLData();

            //TODO: Change hardcode on user id
            string id = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

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
