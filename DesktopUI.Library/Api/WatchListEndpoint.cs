using DesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class WatchListEndpoint: IWatchListEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public WatchListEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<WatchlistModel>> LoadWatchList()
        {
            using(HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/watchlist"))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<WatchlistModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
