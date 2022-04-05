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
using System.Web.Http.Cors;

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
        [Route("id/{id}")]
        public List<PortfolioStockDashboardModel> LoadPortfolioStocksByUserId(string id)
        {
            var portfolioData = new PortfolioData();

            return portfolioData.LoadPortfolioByUserId(id);
        }

        [HttpGet]
        [Route("{ticker}")]
        public PortfolioStockDashboardModel GetPortfolioStock(string ticker)
        {
            var portfolioData = new PortfolioData();

            string id = RequestContext.Principal.Identity.GetUserId();
            //string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return portfolioData.LoadPortfolioStock(id, ticker);
        }

        [HttpPut]
        [Route("buy")]
        public void UpdatePortfolioBuy(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();
            //stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";


            portfolioData.UpdatePortfolioBuy(stock);
        }

        [HttpPut]
        [Route("sell")]
        public decimal UpdatePortfolioSell(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();
            //stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return portfolioData.UpdatePortfolioSell(stock);
        }

        [HttpPost]
        public void PostPortfolio(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();
            //stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            portfolioData.PostStockToPortfolio(stock);
        }

        [HttpPut]
        [Route("{delete}")]
        public decimal UpdateAndDeleteStock(PortfolioModel stock)
        {
            var portfolioData = new PortfolioData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();
            //stock.UserId = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";
            return portfolioData.UpdateAndDeleteStock(stock);
        }
    }
}
