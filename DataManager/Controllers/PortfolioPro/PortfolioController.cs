using DataManager.Library.DataAccess;
using DataManager.Library.Models;
using DataManager.Library.Models.Shared;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataManager.Controllers
{
    [RoutePrefix("api/portfolio")]
    public class PortfolioController : ApiController
    {
        [HttpGet]
        [Route("topholdings")]
        public List<TopHoldingsModel> LoadTopHoldings()
        {
            var portfolioData = new PortfolioData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return portfolioData.LoadTopHoldings(id);
        }

        [HttpGet]
        public List<PortfolioStockDashboardModel> LoadPortfolioStocks()
        {
            var portfolioData = new PortfolioData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return portfolioData.LoadPortfolio(id);
        }

        [HttpGet]
        [Route("{ticker}")]
        public PortfolioStockDashboardModel GetPortfolioStock(string ticker)
        {
            var portfolioData = new PortfolioData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return portfolioData.LoadPortfolioStock(id, ticker);
        }

        [HttpPut]
        [Route("buy")]
        public void UpdatePortfolioBuy(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();

            portfolioData.UpdatePortfolioBuy(stock);
        }

        [HttpPut]
        [Route("sell")]
        public decimal UpdatePortfolioSell(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();

            return portfolioData.UpdatePortfolioSell(stock);
        }

        [HttpPost]
        public void PostPortfolio(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();

            portfolioData.PostStockToPortfolio(stock);
        }

        [HttpPut]
        [Route("{delete}")]
        public decimal UpdateAndDeleteStock(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();

            return portfolioData.UpdateAndDeleteStock(stock);
        }
    }
}
