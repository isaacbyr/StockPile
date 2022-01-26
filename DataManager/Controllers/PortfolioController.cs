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
    [RoutePrefix("api/portfolio")]
    public class PortfolioController : ApiController
    {
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

            //TODO: Remove hardcode of user id
            stock.UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            portfolioData.UpdatePortfolioBuy(stock);
        }

        [HttpPut]
        [Route("sell")]
        public decimal UpdatePortfolioSell(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            //TODO: Remove hardcode of user id
            stock.UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            return portfolioData.UpdatePortfolioSell(stock);
        }

        [HttpPost]
        public void PostPortfolio(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            //TODO: Remove hardcode of user id
            stock.UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c";
            //transaction.UserId = RequestContext.Principal.Identity.GetUserId();

            portfolioData.PostStockToPortfolio(stock);
        }

        [HttpPut]
        [Route("{delete}")]
        public decimal UpdateAndDeleteStock(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            //TODO: Remove hardcode of user id
            stock.UserId = "34b965a6-ba23-4a13-b834-1e456f21d86c";

            return portfolioData.UpdateAndDeleteStock(stock);
        }
    }
}
