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
    [RoutePrefix("api/watchlist")]
    public class WatchlistController : ApiController
    {
        [HttpGet]
        public List<WatchlistModel> LoadWatchlistStock()
        {
            var watchlistData = new WatchlistData();

            string id = RequestContext.Principal.Identity.GetUserId();

            return watchlistData.LoadWatchlistStocksById(id);
        }
    }
}
