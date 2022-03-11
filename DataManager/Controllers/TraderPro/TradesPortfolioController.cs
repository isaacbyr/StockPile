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
    [RoutePrefix("api/tradesportfolio")]
    public class TradesPortfolioController : ApiController
    {

        [HttpGet]
        [Route("{ticker}")]
        public PortfolioStockDashboardModel GetPortfolioStock(string ticker)
        {
            var portfolioData = new TradesPortfolioData();

            //TODO: REMOVE HARDCODE OF USER ID
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return portfolioData.LoadPortfolioStock(id, ticker);
        }

        [HttpPost]
        public void PostPortfolio(PortfolioModel stock)
        {
            var portfolioData = new TradesPortfolioData();

            //TODO: REMOVE HARDCODE OF USER ID
            //stock.UserId = RequestContext.Principal.Identity.GetUserId();
            stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";


            portfolioData.PostStockToPortfolio(stock);
        }

        [HttpPut]
        [Route("buy")]
        public void UpdatePortfolioBuy(PortfolioModel stock)
        {
            var portfolioData = new TradesPortfolioData();

            //TODO: REMOVE HARDCODE OF USER ID
            //stock.UserId = RequestContext.Principal.Identity.GetUserId();
            stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";


            portfolioData.UpdatePortfolioBuy(stock);
        }

        [HttpPut]
        [Route("sell")]
        public decimal UpdatePortfolioSell(PortfolioModel stock)
        {
            var portfolioData = new TradesPortfolioData();

            //TODO: REMOVE HARDCODE OF USER ID
            //stock.UserId = RequestContext.Principal.Identity.GetUserId();
            stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return portfolioData.UpdatePortfolioSell(stock);
        }

        [HttpPut]
        [Route("{delete}")]
        public decimal UpdateAndDeleteStock(PortfolioModel stock)
        {
            var portfolioData = new TradesPortfolioData();

            //TODO: REMOVE HARDCODE OF USER ID
            //stock.UserId = RequestContext.Principal.Identity.GetUserId();
            stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return portfolioData.UpdateAndDeleteStock(stock);
        }
    }
}
