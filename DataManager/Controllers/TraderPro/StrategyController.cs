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
            strategy.UserId = RequestContext.Principal.Identity.GetUserId();

            var strategyData = new StrategyData();

            return strategyData.PostStrategy(strategy);
        }

        [HttpPost]
        [Route("item")]
        public ResponseModel PostStrategyStock(StrategyItemModel strategyStock)
        {
            var strategyData = new StrategyData();

            return strategyData.PostStrategyStock(strategyStock);
        }

        [HttpGet]
        [Route("all")]
        public List<StrategyModel> LoadStrategies()
        {
            var strategyData = new StrategyData();

            // TODO : Change hardcode of id
            string id = RequestContext.Principal.Identity.GetUserId();

            return strategyData.LoadStrategies(id);
        }

        [HttpGet]
        [Route("{id}")]
        public List<StrategyItemModel> GetStrategyStocksById(int id)
        {
            var strategyData = new StrategyData();


            return strategyData.LoadStrategyStocksById(id);
        }
    }
}
