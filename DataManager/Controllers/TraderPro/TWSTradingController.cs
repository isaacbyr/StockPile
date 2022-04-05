using DataManager.Library.DataAccess.TraderPro;
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
    [RoutePrefix("api/twstrading")]
    public class TWSTradingController : ApiController
    {
        
        [HttpPost]
        public void PostTWSStrategy(TWSTradeModel trade)
        {
            var trading = new TWSTradingData();

            trade.UserId = RequestContext.Principal.Identity.GetUserId();
            //trade.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            trading.PostTWSStrategy(trade);
        }

        [HttpGet]
        public List<TWSTradeModel> LoadStrategies()
        {
            var trading = new TWSTradingData();

            string id = RequestContext.Principal.Identity.GetUserId();
            //string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return trading.LoadAllStrategies(id);
        }
    }
}
