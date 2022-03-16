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

            //TODO: REMOVE HARDCODE OF USER ID
            //string id = RequestContext.Principal.Identity.GetUserId();
            string id = "3c0056da-6bfa-40f5-81cf-b0e34b8a198f";

            return watchlistData.LoadWatchlistStocksById(id);
        }

        [HttpPost]
        public ResponseModel PostWatchlistStock(WatchlistModel stock)
        {
            var watchlistData = new WatchlistData();

            stock.UserId = RequestContext.Principal.Identity.GetUserId();

            // check to see if stock is already in db
            var watchlist = watchlistData.LoadWatchlistStocksById(stock.UserId);

            if(watchlist.Any(s => s.Ticker == stock.Ticker))
            {
                var response = new ResponseModel
                {
                    Header = "Watchlist Error",
                    Message = "Stock is already in your watchlist"
                };
                return response;
            }
            else
            {
                watchlistData.PostWatchlistStock(stock);
                var response = new ResponseModel
                {
                    Header = "Watchlist Success",
                    Message = $"{stock.Ticker} was successfully added to your watchlist"
                };
                return response;
            }
        }
    }
}
