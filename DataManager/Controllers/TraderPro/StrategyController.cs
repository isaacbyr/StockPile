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
        [Route("new")]
        public int Post(StrategyModel strategy)
        {
            // TODO CHANGE THIS SHIT
            //strategy.UserId = RequestContext.Principal.Identity.GetUserId();
            strategy.UserId = "123";

            var strategyData = new StrategyData();

            return strategyData.PostStrategy(strategy);
        }

        [HttpPost]
        [Route("stock")]
        public ResponseModel PostStrategyStock(StrategyStockModel strategyStock)
        {
            var strategyData = new StrategyData();

            return strategyData.PostStrategyStock(strategyStock);
        }
    }
}
