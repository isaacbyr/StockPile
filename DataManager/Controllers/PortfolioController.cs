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
    }
}
