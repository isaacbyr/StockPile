using DataManager.Library.DataAccess.TraderPro;
using DataManager.Library.Models;
using DataManager.Library.Models.TraderPro;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers.TraderPro
{
    [RoutePrefix("api/strategy")]
    public class StrategyController : ApiController
    {
        [HttpPost]
        public ResponseModel Post(StrategyModel strategy)
        {
            strategy.UserId = RequestContext.Principal.Identity.GetUserId();

            var strategyData = new StrategyData();

            return strategyData.PostStrategy(strategy);
        }
    }
}
